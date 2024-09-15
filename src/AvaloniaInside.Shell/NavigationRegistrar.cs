using System;
using System.Collections.Generic;
using Avalonia;

namespace AvaloniaInside.Shell;

public class NavigationRegistrar : INavigationRegistrar
{
	private Uri? _defaultRootUri;
	private Dictionary<string, NavigationNode> Navigations { get; } = new();

	public string ApplicationName { get; set; } =
		Application.Current?.Name is not { Length: > 0 } appName
			? "default"
			: appName.Replace(" ", "-").ToLower();

	public Uri RootUri
	{
		get => _defaultRootUri ??= new Uri($"app://{ApplicationName}/");
		set => _defaultRootUri = value;
	}

	public void RegisterRoute(
		string route,
		Type page,
		NavigationNodeType type,
		NavigateType navigate,
		string? defaultPath)
	{
		route = route.ToLower();

		var rootUri = RootUri;
		var newUri = new Uri(rootUri, route);

		if (rootUri.AbsolutePath == newUri.AbsolutePath)
			throw new ArgumentException("Cannot replace the root");
		if (Navigations.ContainsKey(newUri.AbsolutePath))
			throw new ArgumentException("route already exists");

		var node = new NavigationNode(
			newUri.AbsolutePath,
			page,
			type,
			navigate,
			defaultPath);

		var parentPath = newUri.GetParentPath();
		if (parentPath != "/")
		{
			if (!Navigations.TryGetValue(parentPath, out var parent))
				throw new ArgumentException("Cannot find the parent node");

			parent.AddNode(node);
		}

		Navigations[newUri.AbsolutePath] = node;
	}

	public bool TryGetNode(string path, out NavigationNode? node) =>
		Navigations.TryGetValue(path.ToLower(), out node);
}
