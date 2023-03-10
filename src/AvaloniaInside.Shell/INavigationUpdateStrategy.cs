using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public interface INavigationUpdateStrategy
{
	event EventHandler<HostItemChangeEventArgs> HostItemChanged;
	Task UpdateChangesAsync(
		ShellView shellView,
		NavigationStackChanges changes,
		List<object> newInstances,
		NavigateType navigateType,
		object? argument,
		bool hasArgument,
		CancellationToken cancellationToken);
}

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
