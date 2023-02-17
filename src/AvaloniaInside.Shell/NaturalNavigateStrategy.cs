using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class NaturalNavigateStrategy : INavigateStrategy
{
	public virtual Task<Uri> NavigateAsync(NavigationChain chain, Uri currentUri, string path, CancellationToken cancellationToken) =>
		Task.FromResult(new Uri(currentUri, path));

	public virtual Task<Uri?> BackAsync(NavigationChain chain, Uri currentUri, CancellationToken cancellationToken) =>
		Task.FromResult<Uri?>(new Uri(currentUri, ".."));
}
