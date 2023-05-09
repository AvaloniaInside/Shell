using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class GenericPresenter : PresenterBase
{
	public override async Task PresentAsync(ShellView shellView, NavigationChain chain,
        NavigateType navigateType,
        CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		await (shellView.PushViewAsync(
			hostControl ?? chain.Instance,
            navigateType,
            cancellationToken) ?? Task.CompletedTask);

		await (shellView.NavigationBar?.UpdateAsync(chain.Instance, navigateType, cancellationToken) ?? Task.CompletedTask);
	}
}
