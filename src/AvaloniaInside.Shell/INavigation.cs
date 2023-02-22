using System;

namespace AvaloniaInside.Shell;

public interface INavigation
{
	object? Header { get; }
	object? Item { get; }
}
