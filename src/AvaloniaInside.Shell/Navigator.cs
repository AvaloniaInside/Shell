using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class Navigator : INavigator
{
	private readonly INavigateStrategy _navigateStrategy;
	private readonly INavigationUpdateStrategy _updateStrategy;
	private readonly INavigationViewLocator _viewLocator;
	private readonly NavigationStack _stack = new();
	private readonly Dictionary<NavigationChain, TaskCompletionSource<NavigateResult>> _waitingList = new();

	private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
	private bool _navigating;
	private ShellView? _shellView;

	public ShellView ShellView => _shellView ?? throw new ArgumentNullException(nameof(ShellView));

	public Uri CurrentUri => _stack.Current?.Uri ?? Registrar.RootUri;

	public INavigationRegistrar Registrar { get; }

	public Navigator(
		INavigationRegistrar navigationRegistrar,
		INavigateStrategy navigateStrategy,
		INavigationUpdateStrategy updateStrategy,
		INavigationViewLocator viewLocator)
	{
		Registrar = navigationRegistrar;
		_navigateStrategy = navigateStrategy;
		_updateStrategy = updateStrategy;
		_viewLocator = viewLocator;

		_updateStrategy.HostItemChanged += UpdateStrategyOnHostItemChanged;
	}

	public void RegisterShell(ShellView shellView)
	{
		if (_shellView != null) throw new ArgumentException("Register shell can call only once");
		_shellView = shellView;
	}

	public bool HasItemInStack()
	{
		var current = _stack.Current?.Back;
		while (current != null)
		{
			if (current is not HostNavigationChain)
				return true;

			current = current.Back;
		}

		return false;
	}

	public void RegisterPage(string route, Type page, NavigateType navigate) =>
		Registrar.RegisterRoute(route, page, NavigationNodeType.Page, navigate, null);

	public void RegisterHost(string route, Type page, string defaultPath, NavigateType navigate) =>
		Registrar.RegisterRoute(route, page, NavigationNodeType.Host, navigate, defaultPath);

	private async Task NotifyAsync(Uri newUri,
		object? argument,
		bool hasArgument,
		NavigateType? navigateType,
		CancellationToken cancellationToken = default)
	{
		if (!Registrar.TryGetNode(newUri.AbsolutePath, out var node))
		{
			Debug.WriteLine("Warning: Cannot find the path");
			return;
		}

		_navigating = true;

		var instances = new List<object>();
		var finalNavigateType = navigateType ?? node.Navigate;
		var stackChanges = _stack.Push(
			node,
			finalNavigateType,
			newUri,
			instanceFor =>
			{
				var instance = _viewLocator.GetView(instanceFor);
				instances.Add(instance);
				return instance;
			});

		await _updateStrategy.UpdateChangesAsync(
			ShellView,
			stackChanges,
			instances,
			finalNavigateType,
			argument,
			hasArgument,
			cancellationToken);

		CheckWaitingList(stackChanges, argument, hasArgument);

		_navigating = false;
	}

	private async Task SwitchHostedItem(
		NavigationChain old,
		NavigationChain chain,
		CancellationToken cancellationToken = default)
	{
		var newUri =
			await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, chain.Uri.AbsolutePath,
				cancellationToken);
		if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
		{
			await NotifyAsync(newUri, null, false, NavigateType.HostedItemChange, cancellationToken);
		}
	}

	public Task NavigateAsync(string path, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, null, false, cancellationToken);

	public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, argument, true, cancellationToken);

	public Task NavigateAsync(
		string path,
		NavigateType? navigateType,
		CancellationToken cancellationToken = default) =>
		NavigateAsync(path, navigateType, null, false, cancellationToken);

	public Task NavigateAsync(
		string path,
		NavigateType? navigateType,
		object? argument,
		CancellationToken cancellationToken = default) =>
		NavigateAsync(path, navigateType, argument, true, cancellationToken);

	private async Task NavigateAsync(
		string path,
		NavigateType? navigateType,
		object? argument,
		bool hasArgument,
		CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
			if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
				await NotifyAsync(newUri, argument, hasArgument, navigateType, cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public Task BackAsync(CancellationToken cancellationToken = default) =>
		BackAsync(null, false, cancellationToken);

	public Task BackAsync(object? argument, CancellationToken cancellationToken = default) =>
		BackAsync(argument, true, cancellationToken);

	private async Task BackAsync(object? argument, bool hasArgument, CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var newUri = await _navigateStrategy.BackAsync(_stack.Current, CurrentUri, cancellationToken);
			if (newUri != null && CurrentUri.AbsolutePath != newUri.AbsolutePath)
				await NotifyAsync(newUri, argument, hasArgument, NavigateType.Pop, cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public Task<NavigateResult> NavigateAndWaitAsync(string path, CancellationToken cancellationToken = default) =>
		NavigateAndWaitAsync(path, null, null, false, cancellationToken);

	public Task<NavigateResult> NavigateAndWaitAsync(
		string path,
		object? argument,
		CancellationToken cancellationToken = default) =>
		NavigateAndWaitAsync(path, null, argument, true, cancellationToken);

	public Task<NavigateResult> NavigateAndWaitAsync(
		string path,
		NavigateType navigateType,
		CancellationToken cancellationToken = default) =>
		NavigateAndWaitAsync(path, navigateType, null, false, cancellationToken);

	public Task<NavigateResult> NavigateAndWaitAsync(
		string path,
		object? argument,
		NavigateType navigateType,
		CancellationToken cancellationToken = default) =>
		NavigateAndWaitAsync(path, navigateType, argument, true, cancellationToken);

	private async Task<NavigateResult> NavigateAndWaitAsync(
		string path,
		NavigateType? navigateType,
		object? argument,
		bool hasArgument,
		CancellationToken cancellationToken = default)
	{
		var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
		if (CurrentUri.AbsolutePath == newUri.AbsolutePath)
			return new NavigateResult(false, null); // Or maybe we should throw exception.

		await NotifyAsync(newUri, argument, hasArgument, navigateType, cancellationToken);
		var chain = _stack.Current;

		if (!_waitingList.TryGetValue(chain, out var tcs))
			_waitingList[chain] = tcs = new TaskCompletionSource<NavigateResult>();

		try
		{
			return await tcs.Task;
		}
		finally
		{
			_waitingList.Remove(chain);
		}
	}

	private void CheckWaitingList(
		NavigationStackChanges navigationStackChanges,
		object? argument,
		bool hasArgument)
	{
		if (navigationStackChanges.Removed == null) return;
		foreach (var chain in navigationStackChanges.Removed)
		{
			if (_waitingList.TryGetValue(chain, out var tcs))
				tcs.TrySetResult(new NavigateResult(hasArgument, argument));
		}
	}

	private void UpdateStrategyOnHostItemChanged(object? sender, HostItemChangeEventArgs e)
	{
		if (e.OldChain != null && e.NewChain != e.OldChain && !_navigating)
		{
			_ = SwitchHostedItem(e.OldChain, e.NewChain);
		}
	}
}
