using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public interface INavigateStrategy
{
	Task<Uri> NavigateAsync(NavigationChain? chain, Uri currentUri, string path, CancellationToken cancellationToken);
	Task<Uri?> BackAsync(NavigationChain? chain, Uri currentUri, CancellationToken cancellationToken);
}
