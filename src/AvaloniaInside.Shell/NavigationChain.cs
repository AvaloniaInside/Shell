using System;

namespace AvaloniaInside.Shell;

public class NavigationChain
{
	public NavigationNode Node { get; set; } = default!;
	public object Instance { get; set; } = default!;
	public NavigateType Type { get; set; }
	public Uri Uri { get; set; } = default!;
	public NavigationChain? Back { get; set; }
}
