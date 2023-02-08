using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;

public class DefaultNavigationUpdateStrategy : INavigationUpdateStrategy
{
	private readonly IPresenterProvider _presenterProvider;

	public DefaultNavigationUpdateStrategy(IPresenterProvider presenterProvider)
	{
		_presenterProvider = presenterProvider;
	}

	public async Task UpdateChangesAsync(
		NavigationStackChanges changes,
		object? argument,
		CancellationToken cancellationToken)
	{
		var isSame = changes.Previous != changes.Front;

		if (changes.Previous?.Instance is INavigationLifecycle oldInstanceLifecycle && !isSame)
		{
			await oldInstanceLifecycle.StopAsync(cancellationToken);
			_presenterProvider.Pop().Present(changes.Previous);
		}

		if (changes.Removed != null)
			await InvokeRemoveAsync(changes.Removed, changes.Previous, cancellationToken);

		if (changes.Front?.Instance is INavigationLifecycle newInstanceLifecycle)
		{
			if (!isSame)
				await newInstanceLifecycle.StartAsync(cancellationToken);

			if (argument != null)
				await newInstanceLifecycle.ArgumentAsync(argument, cancellationToken);
		}
	}

	private async Task InvokeRemoveAsync(
		IList<NavigationChain> removed,
		NavigationChain? previous,
		CancellationToken cancellationToken)
	{
		var presenter = _presenterProvider.Remove();
		foreach (var chain in removed)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (previous != chain)
				presenter.Present(chain);

			if (chain.Instance is INavigationLifecycle lifecycle)
				await lifecycle.TerminateAsync(cancellationToken);
		}
	}
}
