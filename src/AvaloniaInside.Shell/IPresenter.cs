using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public interface IPresenter
{
	Task PresentAsync(NavigationChain chain, CancellationToken cancellationToken);
}
