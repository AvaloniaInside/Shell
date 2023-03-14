using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class GenericPresenter : PresenterBase
{
	public override async Task PresentAsync(ShellView shellView, NavigationChain chain,
		CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		await (shellView.PushViewAsync(
			hostControl ?? chain.Instance,
			cancellationToken) ?? Task.CompletedTask);

		await (shellView.NavigationBar?.UpdateAsync(chain.Instance, cancellationToken) ?? Task.CompletedTask);
	}
}
