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
		NavigateType navigateType,
		object? argument,
		CancellationToken cancellationToken)
	{
		var isSame = changes.Previous == changes.Front;

		if (changes.Previous?.Instance is INavigationLifecycle oldInstanceLifecycle && !isSame)
			await oldInstanceLifecycle.StopAsync(cancellationToken);

		if (changes.Removed != null)
			await InvokeRemoveAsync(changes.Removed, changes.Previous, cancellationToken);

		if (changes.Front?.Instance is INavigationLifecycle newInstanceLifecycle)
		{
			if (!isSame)
				await newInstanceLifecycle.StartAsync(cancellationToken);

			if (argument != null)
				await newInstanceLifecycle.ArgumentAsync(argument, cancellationToken);
		}

		if (!isSame && navigateType is not NavigateType.Pop && changes.Front != null)
			await _presenterProvider.For(navigateType).PresentAsync(changes.Front, cancellationToken);
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
			if (previous == chain)
				await _presenterProvider.Pop().PresentAsync(previous, cancellationToken);
			else
				await presenter.PresentAsync(chain, cancellationToken);

			if (chain.Instance is INavigationLifecycle lifecycle)
				await lifecycle.TerminateAsync(cancellationToken);
		}
	}
}
