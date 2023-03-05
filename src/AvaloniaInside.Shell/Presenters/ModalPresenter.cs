using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class ModalPresenter : PresenterBase
{

	public override Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		return CurrentShellView?.ModalAsync(
			hostControl ?? chain.Instance,
			cancellationToken) ?? Task.CompletedTask;
	}
}
