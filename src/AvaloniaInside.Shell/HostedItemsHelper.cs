using System;
using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public static class HostedItemsHelper
{
	private class ItemsControlProxy : IHostItems
	{
		private readonly ItemsControl _itemsControl;

		public ItemsControlProxy(ItemsControl itemsControl)
		{
			_itemsControl = itemsControl;
		}

		public IEnumerable? ItemsSource
		{
			get => _itemsControl.ItemsSource;
			set => _itemsControl.ItemsSource = value;
		}

		public ItemCollection Items => _itemsControl.Items;
	}

	private class SelectingItemsControlProxy : ItemsControlProxy, ISelectableHostItems
	{
		private readonly SelectingItemsControl _selectingItemsControl;

		public SelectingItemsControlProxy(SelectingItemsControl itemsControl) : base(itemsControl)
		{
			_selectingItemsControl = itemsControl;
		}

		public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
		{
			add => _selectingItemsControl.SelectionChanged += value;
			remove => _selectingItemsControl.SelectionChanged -= value;
		}

		public object? SelectedItem
		{
			get => _selectingItemsControl.SelectedItem;
			set => _selectingItemsControl.SelectedItem = value;
		}
	}

	public static bool CanBeHosted(Type viewType) =>
		viewType.IsSubclassOf(typeof(ItemsControl)) || typeof(IHostItems).IsAssignableFrom(viewType);

	public static bool CanBeHosted(object view) =>
		view is ItemsControl or SelectingItemsControl or IHostItems or ISelectableHostItems;

	public static IHostItems? GetHostedItems(object? control)
	{
		if (GetSelectableHostedItems(control) is { } casted) return casted;

		if (control is IHostItems hostedItems)
			return hostedItems;
		if (control is ItemsControl itemsControl)
			return new ItemsControlProxy(itemsControl);

		return null;
	}

	public static ISelectableHostItems? GetSelectableHostedItems(object? control)
	{
		if (control is ISelectableHostItems selectableHostedItem)
			return selectableHostedItem;
		if (control is SelectingItemsControl selectingItemsControl)
			return new SelectingItemsControlProxy(selectingItemsControl);

		return null;
	}
}
