using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell.Services;

public interface INavigationService
{
	event EventHandler<NavigateEventArgs> Navigating;
	event EventHandler<NavigateEventArgs> Navigate;

	Task NavigateAsync(string path, CancellationToken cancellationToken = default);
	Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default);
	Task BackAsync(CancellationToken cancellationToken = default);
	Task BackAsync(object? argument, CancellationToken cancellationToken = default);
}
