using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class NormalPresenter : PresenterBase
{
	public override Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken) =>
		CurrentShellView?.PushViewAsync(chain.Instance, cancellationToken) ?? Task.CompletedTask;
}