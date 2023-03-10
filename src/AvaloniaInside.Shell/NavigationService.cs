using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class NavigationService : INavigationService
{
	private readonly INavigationRegistrar _navigationRegistrar;
	private readonly INavigateStrategy _navigateStrategy;
	private readonly INavigationUpdateStrategy _updateStrategy;
	private readonly INavigationViewLocator _viewLocator;
	private readonly NavigationStack _stack = new();

	private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
	private bool _navigating;
	private ShellView? _shellView;

	public ShellView ShellView => _shellView ?? throw new ArgumentNullException(nameof (ShellView));

	public Uri CurrentUri => _stack.Current?.Uri ?? _navigationRegistrar.RootUri;

	public NavigationService(
		INavigationRegistrar navigationRegistrar,
		INavigateStrategy navigateStrategy,
		INavigationUpdateStrategy updateStrategy,
		INavigationViewLocator viewLocator)
	{
		_navigationRegistrar = navigationRegistrar;
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
		do
		{
			if (current is not HostNavigationChain)
				return true;

			current = current.Back;
		} while (current != null);

		return false;
	}

	public void RegisterPage(string route, Type page, NavigateType navigate) =>
		_navigationRegistrar.RegisterRoute(route, page, NavigationNodeType.Page, navigate, null);

	public void RegisterHost(string route, Type page, string defaultPath, NavigateType navigate) =>
		_navigationRegistrar.RegisterRoute(route, page, NavigationNodeType.Host, navigate, defaultPath);

	private async Task NotifyAsync(
		Uri newUri,
		object? argument,
		NavigateType? navigateType,
		CancellationToken cancellationToken = default)
	{
		if (!_navigationRegistrar.TryGetNode(newUri.AbsolutePath, out var node))
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
			cancellationToken);

		_navigating = false;
	}

	private async Task SwitchHostedItem(
		NavigationChain old,
		NavigationChain chain,
		CancellationToken cancellationToken = default)
	{
		var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, chain.Uri.AbsolutePath, cancellationToken);
		if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
		{
			await NotifyAsync(newUri, null, NavigateType.HostedItemChange, cancellationToken);
		}
	}

	public Task NavigateAsync(string path, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, null, cancellationToken);

	public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, argument, cancellationToken);

	public async Task NavigateAsync(string path, NavigateType? navigateType, object? argument,
		CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
			if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
				await NotifyAsync(newUri, argument, navigateType, cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
		}
	}

	public Task BackAsync(CancellationToken cancellationToken = default) =>
		BackAsync(null, cancellationToken);

	public async Task BackAsync(object? argument, CancellationToken cancellationToken = default)
	{
		await _semaphoreSlim.WaitAsync(cancellationToken);
		try
		{
			var newUri = await _navigateStrategy.BackAsync(_stack.Current, CurrentUri, cancellationToken);
			if (newUri != null && CurrentUri.AbsolutePath != newUri.AbsolutePath)
				await NotifyAsync(newUri, argument, NavigateType.Pop, cancellationToken);
		}
		finally
		{
			_semaphoreSlim.Release();
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
