using System;

namespace AvaloniaInside.Shell.Services;

public class NavigateEventArgs : EventArgs
{
	public NavigateEventArgs(NavigationNode item, Uri old, Uri uri, object? argument)
	{
		Node = item;
		Uri = uri;
		OldUri = old;
		Argument = argument;
	}

	public NavigationNode Node { get; }
	public Uri OldUri { get; }
	public Uri Uri { get; }
	public object? Argument { get; }
	public bool Handled { get; set; } = false;

	public T? As<T>() => Argument is T argument ? argument : default;
}
