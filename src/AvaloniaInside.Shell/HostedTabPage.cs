using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace AvaloniaInside.Shell;

public class HostedTabPage : Page, ISelectableHostedItems
{
	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

	protected override Type StyleKeyOverride => typeof(HostedTabPage);

	#region ItemTemplate

	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
		AvaloniaProperty.Register<HostedTabPage, IDataTemplate?>(nameof(ItemTemplate));

	public IDataTemplate? ItemTemplate
	{
		get => GetValue(ItemTemplateProperty);
		set => SetValue(ItemTemplateProperty, value);
	}

	#endregion

	#region ItemsSource

	public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
		AvaloniaProperty.Register<ItemsControl, IEnumerable?>(nameof(ItemsSource));

	public IEnumerable? ItemsSource
	{
		get => GetValue(ItemsSourceProperty);
		set => SetValue(ItemsSourceProperty, value);
	}

#pragma warning disable CS8603 // Possible null reference return.
	public ItemCollection Items => null;
#pragma warning restore CS8603 // Possible null reference return.

	#endregion

	#region SelectedIndex

	public static readonly DirectProperty<HostedTabPage, int> SelectedIndexProperty =
		AvaloniaProperty.RegisterDirect<HostedTabPage, int>(
			nameof(SelectedIndex),
			o => o.SelectedIndex,
			(o, v) => o.SelectedIndex = v,
			unsetValue: -1,
			defaultBindingMode: BindingMode.TwoWay);

	private int _selectedIndex;
	public int SelectedIndex
	{
		get => _selectedIndex;
		set
		{
			if (ItemsSource is not { } itemsSource) return;

			if (itemsSource.Cast<object>().Skip(value).FirstOrDefault() is not { } found)
				throw new IndexOutOfRangeException($"{value} out of index");

			if (SetAndRaise(SelectedIndexProperty, ref _selectedIndex, value))
			{
				SelectedItem = found;
			}
		}
	}

	#endregion

	#region SelectedItem

	public static readonly DirectProperty<HostedTabPage, object?> SelectedItemProperty =
		AvaloniaProperty.RegisterDirect<HostedTabPage, object?>(
			nameof(SelectedItem),
			o => o.SelectedItem,
			(o, v) => o.SelectedItem = v,
			defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

	private object? _selectedItem;

	public object? SelectedItem
	{
		get => _selectedItem;
		set
		{
			var current = _selectedItem;
			if (SetAndRaise(SelectedItemProperty, ref _selectedItem, value))
				SelectionChanged?.Invoke(
					this,
					new SelectionChangedEventArgs(
						null,
						current == null ? [] : new List<object> { current },
						value == null ? [] : new List<object> { value }));
		}
	}

	#endregion

	#region SelectedContentTemplate

	public static readonly DirectProperty<HostedTabPage, IDataTemplate?> SelectedContentTemplateProperty =
		AvaloniaProperty.RegisterDirect<HostedTabPage, IDataTemplate?>(
			nameof(SelectedContentTemplate),
			o => o.SelectedContentTemplate,
			(o, v) => o.SelectedContentTemplate = v);

	private IDataTemplate? _selectedContentTemplate;

	public IDataTemplate? SelectedContentTemplate
	{
		get => _selectedContentTemplate;
		set => SetAndRaise(SelectedContentTemplateProperty, ref _selectedContentTemplate, value);
	}

	#endregion
}
