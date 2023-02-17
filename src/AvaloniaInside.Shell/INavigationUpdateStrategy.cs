using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace AvaloniaInside.Shell;

public interface INavigationUpdateStrategy
{
	Task UpdateChangesAsync(
		NavigationStackChanges changes,
		NavigateType navigateType,
		object? argument,
		CancellationToken cancellationToken);
}
