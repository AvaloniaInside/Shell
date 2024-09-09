using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AvaloniaInside.Shell;

[TemplatePart("PART_TabStripPlaceHolder", typeof(ContentPresenter))]
[TemplatePart("PART_Carousel", typeof(Carousel))]
public class TabPage : Page, ISelectableHostItems
{
	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

	private ContentPresenter? _tabStripPlaceHolder;

	protected override Type StyleKeyOverride => typeof(TabPage);

	#region TabStripTemplate

	public static readonly StyledProperty<IControlTemplate?> TabStripTemplateProperty =
		AvaloniaProperty.Register<TabPage, IControlTemplate?>(nameof(TabStripTemplate));

	public IControlTemplate? TabStripTemplate
	{
		get => GetValue(TabStripTemplateProperty);
		set => SetValue(TabStripTemplateProperty, value);
	}

	#endregion

	#region ItemTemplate

	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
		AvaloniaProperty.Register<TabPage, IDataTemplate?>(nameof(ItemTemplate));

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

	public static readonly DirectProperty<TabPage, int> SelectedIndexProperty =
		AvaloniaProperty.RegisterDirect<TabPage, int>(
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

	public static readonly DirectProperty<TabPage, object?> SelectedItemProperty =
		AvaloniaProperty.RegisterDirect<TabPage, object?>(
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
			{
				SelectionChanged?.Invoke(
					this,
					new SelectionChangedEventArgs(
						null,
						current == null ? [] : new List<object> { current },
						value == null ? [] : new List<object> { value }));

				if (AttachedNavigationBar is { } navBar && value is NavigationChain chain)
					navBar.UpdateView(chain.Instance);
			}
		}
	}

	#endregion

	#region SelectedContentTemplate

	public static readonly DirectProperty<TabPage, IDataTemplate?> SelectedContentTemplateProperty =
		AvaloniaProperty.RegisterDirect<TabPage, IDataTemplate?>(
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

	#region TabStripPlacement

	/// <summary>
	/// Defines the <see cref="TabStripPlacement"/> property.
	/// </summary>
	public static readonly StyledProperty<Dock> TabStripPlacementProperty =
		AvaloniaProperty.Register<TabControl, Dock>(nameof(TabStripPlacement), defaultValue: Dock.Bottom);

	/// <summary>
	/// Gets or sets the tabstrip placement of the TabControl.
	/// </summary>
	public Dock TabStripPlacement
	{
		get => GetValue(TabStripPlacementProperty);
		set => SetValue(TabStripPlacementProperty, value);
	}

	#endregion

	#region ItemsPanel

	private static readonly FuncTemplate<Panel?> DefaultPanel =
		new(() => new StackPanel
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			Orientation = Orientation.Horizontal
		});

	/// <summary>
	/// Defines the <see cref="ItemsPanel"/> property.
	/// </summary>
	public static readonly StyledProperty<ITemplate<Panel?>> ItemsPanelProperty =
		AvaloniaProperty.Register<ItemsControl, ITemplate<Panel?>>(nameof(ItemsPanel), DefaultPanel);

	/// <summary>
	/// Gets or sets the panel used to display the items.
	/// </summary>
	public ITemplate<Panel?> ItemsPanel
	{
		get => GetValue(ItemsPanelProperty);
		set => SetValue(ItemsPanelProperty, value);
	}

	#endregion

	#region Setup and Theme

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == TabStripTemplateProperty)
		{
			ApplyTabStripTemplate();
		}
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		if (AttachedNavigationBar is { } navBar && SelectedItem is NavigationChain chain)
			navBar.UpdateView(chain.Instance);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_tabStripPlaceHolder = e.NameScope.Find<ContentPresenter>("PART_TabStripPlaceHolder");

		ApplyTabStripTemplate();
	}

	protected virtual void ApplyTabStripTemplate()
	{
		if (TabStripTemplate is not { } template || _tabStripPlaceHolder is not { } tabStripPlaceHolder) return;

		//Logger.TryGet(LogEventLevel.Verbose, LogArea.Control)?.Log(this, "Creating control template");

		if (template.Build(this) is { } templateResult)
		{
			var (child, nameScope) = templateResult;
			((ISetLogicalParent)child).SetParent(this);

			//HACK using
			SetPrivateDateTimePropertyValue(child, "TemplatedParent", this);
			child.ApplyTemplate();

			tabStripPlaceHolder.Content = child;
		}
	}

	static void SetPrivateDateTimePropertyValue<T>(T member, string propName, object newValue)
	{
		PropertyInfo propertyInfo = typeof(T).GetProperty(propName);
		if (propertyInfo == null) return;
		propertyInfo.SetValue(member, newValue);
	}

	#endregion

	#region Sizing

	protected override void UpdateSafePaddingSizes()
	{
		TopSafeSpace = SafePadding.Top;
		TopSafePadding = new Thickness(0, SafePadding.Top, 0, 0);
		BottomSafeSpace = SafePadding.Bottom;
		BottomSafePadding = new Thickness(0, 0, 0, SafePadding.Bottom);
		LeftSafeSpace = SafePadding.Left;
		LeftSafePadding = new Thickness(SafePadding.Left, 0, 0, 0);
		RightSafeSpace = SafePadding.Right;
		RightSafePadding = new Thickness(0, 0, SafePadding.Right, 0);

		PageSafePadding  = new Thickness(
			TabStripPlacement != Dock.Left ? SafePadding.Left : 0,
			0,
			TabStripPlacement != Dock.Right ? SafePadding.Right : 0,
			TabStripPlacement != Dock.Bottom ? SafePadding.Bottom : 0);

		TabSafePadding  = new Thickness(
			TabStripPlacement != Dock.Right ? SafePadding.Left : 0,
			0,
			TabStripPlacement != Dock.Left ? SafePadding.Right : 0,
			TabStripPlacement != Dock.Top ? SafePadding.Bottom : 0);
	}

	#endregion
}
