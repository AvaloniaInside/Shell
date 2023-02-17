using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class RemovePresenter : PresenterBase
{
	public override Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken) =>
		CurrentShellView?.RemoveViewAsync(chain.Instance, cancellationToken) ?? Task.CompletedTask;
}