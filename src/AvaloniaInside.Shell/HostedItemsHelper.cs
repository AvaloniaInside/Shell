using System;
using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public static class HostedItemsHelper
{
	private class ItemsControlProxy(ItemsControl itemsControl) : IHostedItems
	{
		public IEnumerable? ItemsSource
		{
			get => itemsControl.ItemsSource;
			set => itemsControl.ItemsSource = value;
		}

		public ItemCollection Items => itemsControl.Items;
	}

	private class SelectingItemsControlProxy(SelectingItemsControl itemsControl)
		: ItemsControlProxy(itemsControl), ISelectableHostedItems
	{
		public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
		{
			add => itemsControl.SelectionChanged += value;
			remove => itemsControl.SelectionChanged -= value;
		}

		public object? SelectedItem
		{
			get => itemsControl.SelectedItem;
			set => itemsControl.SelectedItem = value;
		}
	}

	public static bool CanBeHosted(Type viewType) =>
		viewType.IsSubclassOf(typeof(ItemsControl)) || typeof(IHostedItems).IsAssignableFrom(viewType);

	public static bool CanBeHosted(object view) =>
		view is ItemsControl or SelectingItemsControl or IHostedItems or ISelectableHostedItems;

	public static IHostedItems? GetHostedItems(object? control)
	{
		if (GetSelectableHostedItems(control) is { } casted) return casted;

		if (control is IHostedItems hostedItems)
			return hostedItems;
		if (control is ItemsControl itemsControl)
			return new ItemsControlProxy(itemsControl);

		return null;
	}

	public static ISelectableHostedItems? GetSelectableHostedItems(object? control)
	{
		if (control is ISelectableHostedItems selectableHostedItem)
			return selectableHostedItem;
		if (control is SelectingItemsControl selectingItemsControl)
			return new SelectingItemsControlProxy(selectingItemsControl);

		return null;
	}
}
