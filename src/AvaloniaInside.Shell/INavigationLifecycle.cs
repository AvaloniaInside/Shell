using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public interface INavigationLifecycle
{
	Task InitialiseAsync(CancellationToken cancellationToken);
	Task StartAsync(CancellationToken cancellationToken);
	Task StopAsync(CancellationToken cancellationToken);
	Task ArgumentAsync(object args, CancellationToken cancellationToken);
	Task TerminateAsync(CancellationToken cancellationToken);
}
