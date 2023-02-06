using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using AvaloniaInside.Shell.Views;

namespace AvaloniaInside.Shell.Services;

public class NavigationService : INavigationService
{
	public event EventHandler<NavigateEventArgs> Navigating;
	public event EventHandler<NavigateEventArgs> Navigate;

	public Dictionary<string, NavigationNode> Navigations { get; } = new();
	public Uri CurrentUri { get; private set; }
	public NavigationNode CurrentItem { get; private set; }

	public NavigationService()
	{
		CurrentUri = new Uri($"app://{GetAppName()}");
	}

	private static string GetAppName() =>
		Application.Current?.Name is not { Length: > 0 } appName
			? "default"
			: appName.Replace(" ", "-").ToLower();

	public void RegisterRoute(string route, Type page, NavigationNodeType type)
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
			type);

		var parentUri = new Uri(newUri, "..");
		if (parentUri.AbsolutePath != "/")
		{
			if (!Navigations.TryGetValue(parentUri.AbsolutePath, out var parent))
				throw new ArgumentException("Cannot find the parent node");

			parent.AddNode(node);
		}

		Navigations[newUri.AbsolutePath] = node;
	}

	private Task NotifyAsync(Uri old, Uri newUri, object? argument, CancellationToken cancellationToken = default)
	{
		if (!Navigations.TryGetValue(newUri.AbsolutePath, out var navigationItem))
		{
			Debug.WriteLine("Warning: Cannot find the path");
			return Task.CompletedTask;
		}

		var navigate = new NavigateEventArgs(navigationItem, old, CurrentUri, argument);

		OnNavigating(navigate);
		if (!navigate.Handled)
		{
			CurrentItem = navigationItem;
			CurrentUri = newUri;

			OnNavigate(navigate);
		}

		return Task.CompletedTask;
	}

	public Task NavigateAsync(string path, CancellationToken cancellationToken = default) =>
		NavigateAsync(path, null, cancellationToken);
	public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default)
	{
		var newUri = new Uri(CurrentUri, path);
		return CurrentUri.AbsolutePath == newUri.AbsolutePath
			? Task.CompletedTask
			: NotifyAsync(CurrentUri, newUri, argument, cancellationToken);
	}

	public Task BackAsync(CancellationToken cancellationToken = default) =>
		BackAsync(null, cancellationToken);
	public Task BackAsync(object? argument, CancellationToken cancellationToken = default)
	{
		return NavigateAsync("..", argument, cancellationToken);
	}

	protected virtual void OnNavigating(NavigateEventArgs e) => Navigating?.Invoke(this, e);
	protected virtual void OnNavigate(NavigateEventArgs e) => Navigate?.Invoke(this, e);

}
