using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Presenters;

public abstract class PresenterBase : IPresenter
{
	protected ShellView? CurrentShellView => ShellView.Current;
	public abstract Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken);
}