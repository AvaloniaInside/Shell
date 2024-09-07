using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationStackChanges
{
	public NavigationChain? Previous { get; set; }
	public NavigationChain? Front { get; set; }
	public IList<NavigationChain>? Removed { get; set; }
	public IList<NavigationChain> NewNavigationChains { get; set; } = [];
}
