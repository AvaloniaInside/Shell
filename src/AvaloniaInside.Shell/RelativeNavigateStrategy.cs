using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class RelativeNavigateStrategy : NaturalNavigateStrategy
{
	public override Task<Uri?> BackAsync(NavigationChain chain, Uri currentUri, CancellationToken cancellationToken) =>
		Task.FromResult(chain.Back?.Uri);
}
