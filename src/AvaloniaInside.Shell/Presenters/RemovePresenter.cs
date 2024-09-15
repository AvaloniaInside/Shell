using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class RemovePresenter : PresenterBase
{
	public override Task PresentAsync(
		ShellView shellView,
        NavigationChain chain,
        NavigateType navigateType,
        NavigateEventArgs eventArgs,
        CancellationToken cancellationToken) =>
		shellView.RemoveViewAsync(chain.Instance, navigateType, eventArgs, cancellationToken);
}
