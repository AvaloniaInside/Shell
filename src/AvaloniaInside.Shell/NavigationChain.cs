namespace AvaloniaInside.Shell;

public class NavigationChain
{
	public NavigationNode Node { get; set; }
	public object Instance { get; set; }
	public NavigateType Type { get; set; }

	public NavigationChain? Back { get; set; }
}