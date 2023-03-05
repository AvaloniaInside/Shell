using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public class NormalPresenter : PresenterBase
{
	public override async Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		await (CurrentShellView?.PushViewAsync(
			hostControl ?? chain.Instance,
			cancellationToken) ?? Task.CompletedTask);

		if (hostControl != null && hostControl != chain.Instance)
			await (CurrentShellView?.NavigationView?.UpdateAsync(chain.Instance, cancellationToken) ?? Task.CompletedTask);
	}
}
