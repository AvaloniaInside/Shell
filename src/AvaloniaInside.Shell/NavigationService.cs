using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;

namespace AvaloniaInside.Shell;

public class NavigationService : INavigationService
{
	private readonly INavigationViewLocator _viewLocator;
	private readonly INavigationUpdateStrategy _updateStrategy;
	private readonly NavigationStack _stack = new();

	public event EventHandler<object> Add;
	public event EventHandler<object> Remove;
	public event EventHandler<NavigateEventArgs> Navigating;
	public event EventHandler<NavigateEventArgs> Navigate;

	private Dictionary<string, NavigationNode> Navigations { get; } = new();
	public Uri CurrentUri { get; private set; }
	public NavigationNode CurrentNode { get; private set; }

	public NavigationService(INavigationViewLocator viewLocator, INavigationUpdateStrategy updateStrategy)
	{
		_viewLocator = viewLocator;
		_updateStrategy = updateStrategy;
		CurrentUri = new Uri($"app://{GetAppName()}");
	}

	private static string GetAppName() =>
		Application.Current?.Name is not { Length: > 0 } appName
			? "default"
			: appName.Replace(" ", "-").ToLower();

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
		Uri old,
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

		var navigate = new NavigateEventArgs(node, old, CurrentUri, argument);

		OnNavigating(navigate);
		if (navigate.Cancel) return;

		CurrentNode = node;
		CurrentUri = newUri;

		object? instance = null;
		var stackChanges = _stack.Push(
			node,
			navigateType ?? node.Navigate,
			() => instance = _viewLocator.GetView(node));

		if (instance is INavigationLifecycle oldInstanceLifecycle)
			await oldInstanceLifecycle.InitialiseAsync(cancellationToken);

		await _updateStrategy.UpdateChangesAsync(
			stackChanges,
			argument,
			cancellationToken);

		OnNavigate(navigate);
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
			: NotifyAsync(CurrentUri, newUri, argument, navigateType, cancellationToken);
	}

	public Task BackAsync(CancellationToken cancellationToken = default) =>
		BackAsync(null, cancellationToken);

	public Task BackAsync(object? argument, CancellationToken cancellationToken = default) =>
		NavigateAsync("..", NavigateType.Pop, argument, cancellationToken);

	protected virtual void OnNavigating(NavigateEventArgs e) => Navigating?.Invoke(this, e);
	protected virtual void OnNavigate(NavigateEventArgs e) => Navigate?.Invoke(this, e);
}
