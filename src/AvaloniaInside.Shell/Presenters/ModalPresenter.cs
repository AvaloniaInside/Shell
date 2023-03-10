using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class ModalPresenter : PresenterBase
{

	public override Task PresentAsync(ShellView shellView, NavigationChain chain, CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		return shellView.ModalAsync(
			hostControl ?? chain.Instance,
			cancellationToken) ?? Task.CompletedTask;
	}
}
