using System;

namespace AvaloniaInside.Shell;

public interface INavigationRegistrar
{
	string ApplicationName { get; set; }
	Uri RootUri { get; set; }

	void RegisterRoute(
		string route,
		Type page,
		NavigationNodeType type,
		NavigateType navigate,
		string? defaultPath);

	bool TryGetNode(string path, out NavigationNode? node);
}
