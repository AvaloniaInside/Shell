using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public interface INavigationUpdateStrategy
{
	event EventHandler<HostItemChangeEventArgs> HostItemChanged;

	Task UpdateChangesAsync(
		ShellView shellView,
		NavigationStackChanges changes,
		NavigateType navigateType,
		object? argument,
		bool hasArgument,
		NavigateEventArgs eventArgs,
		CancellationToken cancellationToken);
}
