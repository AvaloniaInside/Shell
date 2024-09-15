using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class GenericPresenter : PresenterBase
{
	public override async Task PresentAsync(
		ShellView shellView,
		NavigationChain chain,
        NavigateType navigateType,
        NavigateEventArgs eventArgs,
        CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		await shellView.PushViewAsync(
			hostControl ?? chain.Instance,
            navigateType,
			eventArgs,
            cancellationToken);
	}
}
