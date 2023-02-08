using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationStackChanges
{
	public IList<NavigationChain>? New { get; set; }
	public IList<NavigationChain>? Removed { get; set; }
}