using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class ModalPresenter : PresenterBase
{

	public override Task PresentAsync(
		ShellView shellView,
        NavigationChain chain,
        NavigateType navigateType,
        NavigateEventArgs eventArgs,
        CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		return shellView.ModalAsync(
			hostControl ?? chain.Instance,
            navigateType,
            eventArgs,
            cancellationToken) ?? Task.CompletedTask;
	}
}
