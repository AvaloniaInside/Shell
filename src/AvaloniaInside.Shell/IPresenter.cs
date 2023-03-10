using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public interface IPresenter
{
	Task PresentAsync(ShellView shellView, NavigationChain chain, CancellationToken cancellationToken);
}
