using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;

namespace AvaloniaInside.Shell;

public class NavigationService : INavigationService
{
	private readonly INavigateStrategy _navigateStrategy;
	private readonly INavigationUpdateStrategy _updateStrategy;
	private readonly INavigationViewLocator _viewLocator;
	private readonly NavigationStack _stack = new();

	private Dictionary<string, NavigationNode> Navigations { get; } = new();
	public Uri CurrentUri => _stack.Current?.Uri ?? GetRootUri();

	public NavigationService(INavigateStrategy navigateStrategy, INavigationUpdateStrategy updateStrategy, INavigationViewLocator viewLocator)
	{
		_navigateStrategy = navigateStrategy;
		_updateStrategy = updateStrategy;
		_viewLocator = viewLocator;
	}

	private static string GetAppName() =>
		Application.Current?.Name is not { Length: > 0 } appName
			? "default"
			: appName.Replace(" ", "-").ToLower();

	private static Uri GetRootUri() =>
		new Uri($"app://{GetAppName()}/");

	public bool HasItemInStack() => _stack.Current?.Back != null;

	public void RegisterRoute(string route, Type page, NavigationNodeType type, NavigateType navigate)
	{
		route = route.ToLower();

		var rootUri = new Uri(CurrentUri, "/");
		var newUri = new Uri(rootUri, route);

		if (rootUri.AbsolutePath == newUri.AbsolutePath)
			throw new ArgumentException("Cannot replace the root");
		if (Navigations.ContainsKey(newUri.AbsolutePath))
			throw new ArgumentException("route already exists");

		var node = new NavigationNode(
			newUri.AbsolutePath,
			page,
			type,
			navigate);

		var parentUri = new Uri(newUri, "..");
		if (parentUri.AbsolutePath != "/")
		{
			if (!Navigations.TryGetValue(parentUri.AbsolutePath, out var parent))
				throw new ArgumentException("Cannot find the parent node");

			parent.AddNode(node);
		}

		Navigations[newUri.AbsolutePath] = node;
	}

	private async Task NotifyAsync(
		Uri newUri,
		object? argument,
		NavigateType? navigateType,
		CancellationToken cancellationToken = default)
	{
		if (!Navigations.TryGetValue(newUri.AbsolutePath, out var node))
		{
			Debug.WriteLine("Warning: Cannot find the path");
			return;
		}

		object? instance = null;
		var finalNavigateType = navigateType ?? node.Navigate;
		var stackChanges = _stack.Push(
			node,
			finalNavigateType,
			newUri,
			() => instance = _viewLocator.GetView(node));

		if (instance is INavigationLifecycle oldInstanceLifecycle)
			await oldInstanceLifecycle.InitialiseAsync(cancellationToken);

		await _updateStrategy.UpdateChangesAsync(
			stackChanges,
			finalNavigateType,
			argument,
			cancellationToken);
	}

	public Task NavigateAsync(string path, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, null, cancellationToken);

	public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, argument, cancellationToken);

	public Task NavigateAsync(string path, NavigateType? navigateType, object? argument, CancellationToken cancellationToken = default)
	{
		var newUri = new Uri(CurrentUri, path);
		return CurrentUri.AbsolutePath == newUri.AbsolutePath
			? Task.CompletedTask
			: NotifyAsync(newUri, argument, navigateType, cancellationToken);
	}

	public Task BackAsync(CancellationToken cancellationToken = default) =>
		BackAsync(null, cancellationToken);

	public async Task BackAsync(object? argument, CancellationToken cancellationToken = default)
	{
		var newUri = await _navigateStrategy.BackAsync(_stack.Current, CurrentUri, cancellationToken);
		if (newUri != null && CurrentUri.AbsolutePath != newUri.AbsolutePath)
			await NotifyAsync(newUri, argument, NavigateType.Pop, cancellationToken);
	}
}
