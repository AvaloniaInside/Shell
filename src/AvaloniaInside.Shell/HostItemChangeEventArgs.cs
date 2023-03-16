namespace AvaloniaInside.Shell;

public class HostItemChangeEventArgs
{
	public HostItemChangeEventArgs(NavigationChain? oldChain, NavigationChain newChain)
	{
		OldChain = oldChain;
		NewChain = newChain;
	}

	public NavigationChain? OldChain { get; }
	public NavigationChain NewChain { get; }
}