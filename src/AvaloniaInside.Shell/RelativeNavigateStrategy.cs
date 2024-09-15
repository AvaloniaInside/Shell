using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class RelativeNavigateStrategy : NaturalNavigateStrategy
{
	public RelativeNavigateStrategy(INavigationRegistrar navigationRegistrar) : base(navigationRegistrar)
	{
	}

	public override Task<Uri?> BackAsync(NavigationChain? chain, Uri currentUri, CancellationToken cancellationToken)
	{
		var current = chain?.Back;
		while (current is HostNavigationChain host)
			current = host.Back;

		return Task.FromResult(current?.Uri);
	}
}
