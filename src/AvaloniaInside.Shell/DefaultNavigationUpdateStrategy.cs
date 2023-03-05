using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class DefaultNavigationUpdateStrategy : INavigationUpdateStrategy
{
	private readonly IPresenterProvider _presenterProvider;

	public DefaultNavigationUpdateStrategy(IPresenterProvider presenterProvider)
	{
		_presenterProvider = presenterProvider;
	}

	public event EventHandler<HostItemChangeEventArgs>? HostItemChanged;

	public async Task UpdateChangesAsync(
		NavigationStackChanges changes,
		List<object> newInstances,
		NavigateType navigateType,
		object? argument,
		CancellationToken cancellationToken)
	{
		var isSame = changes.Previous == changes.Front;

		foreach (var instance in newInstances)
		{
			if (instance is INavigationLifecycle navigationLifecycle)
				await navigationLifecycle.InitialiseAsync(cancellationToken);

			SubscribeForUpdateIfNeeded(instance);
		}

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

		if (!isSame && changes.Front != null)
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
				await _presenterProvider.Remove().PresentAsync(previous, cancellationToken);
			else
				await presenter.PresentAsync(chain, cancellationToken);

			if (chain.Instance is INavigationLifecycle lifecycle)
				await lifecycle.TerminateAsync(cancellationToken);

			UnSubscribeForUpdateIfNeeded(chain.Instance);
		}
	}

	private void SubscribeForUpdateIfNeeded(object? instance)
	{
		if (instance is not SelectingItemsControl selectingItemsControl) return;
		selectingItemsControl.SelectionChanged += SelectingItemsControlOnSelectionChanged;
	}

	private void UnSubscribeForUpdateIfNeeded(object instance)
	{
		if (instance is not SelectingItemsControl selectingItemsControl) return;
		selectingItemsControl.SelectionChanged -= SelectingItemsControlOnSelectionChanged;
	}

	private void SelectingItemsControlOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems?.Count > 0 && e.AddedItems[0] is NavigationChain chain)
		{
			HostItemChanged?.Invoke(this, new HostItemChangeEventArgs(
				e.RemovedItems?.Count > 0 ? e.RemovedItems[0] as NavigationChain : null,
				chain));
		}
	}
}
