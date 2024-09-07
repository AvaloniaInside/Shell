using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaInside.Shell;

[TemplatePart("PART_Header", typeof(ContentControl))]
[TemplatePart("PART_ActionButton", typeof(Button))]
[TemplatePart("PART_Items", typeof(ContentControl))]
public class NavigationBar : TemplatedControl
{
	private ContentControl? _header;
	private Button? _actionButton;
	private ContentControl? _items;

	private object? _pendingHeader;

	#region ShellView

    public static readonly StyledProperty<ShellView?> ShellViewProperty =
	    AvaloniaProperty.Register<NavigationBar, ShellView?>(nameof(ShellView));

    public ShellView? ShellView
    {
	    get => GetValue(ShellViewProperty);
	    internal set => SetValue(ShellViewProperty, value);
    }

    #endregion

    #region BackCommand

    public static readonly DirectProperty<NavigationBar, ICommand?> BackCommandProperty =
	    AvaloniaProperty.RegisterDirect<NavigationBar, ICommand?>(
		    nameof(BackCommand),
		    o => o.BackCommand,
		    (o, v) => o.BackCommand = v);

	private ICommand? _backCommand;

	public ICommand? BackCommand
	{
		get => _backCommand;
		set
		{
			if (SetAndRaise(BackCommandProperty, ref _backCommand, value))
				UpdateButtons();
		}
	}

	#endregion

	#region SideMenuCommand

	public static readonly DirectProperty<NavigationBar, ICommand?> SideMenuCommandProperty =
		AvaloniaProperty.RegisterDirect<NavigationBar, ICommand?>(
			nameof(SideMenuCommand),
			o => o.SideMenuCommand,
			(o, v) => o.SideMenuCommand = v);

	private ICommand? _sideMenuCommand;

	public ICommand? SideMenuCommand
	{
		get => _sideMenuCommand;
		set
		{
			if (SetAndRaise(SideMenuCommandProperty, ref _sideMenuCommand, value))
				UpdateButtons();
		}
	}

	#endregion

	#region HasSideMenuOption

	public static readonly DirectProperty<NavigationBar, bool> HasSideMenuOptionProperty =
		AvaloniaProperty.RegisterDirect<NavigationBar, bool>(
			nameof(HasSideMenuOption),
			o => o.HasSideMenuOption,
			(o, v) => o.HasSideMenuOption = v);

	private bool _hasSideMenuOption = true;

	public bool HasSideMenuOption
	{
		get => _hasSideMenuOption;
		set
		{
			if (SetAndRaise(HasSideMenuOptionProperty, ref _hasSideMenuOption, value))
				UpdateButtons();
		}
	}

	#endregion

	#region CurrentView

	public static readonly DirectProperty<NavigationBar, object?> CurrentViewProperty =
		AvaloniaProperty.RegisterDirect<NavigationBar, object?>(
			nameof(CurrentView),
			o => o.CurrentView,
			(o, v) => o.CurrentView = v);

	private object? _currentView;
	public object? CurrentView
	{
		get => _currentView;
		set
		{
			if (SetAndRaise(CurrentViewProperty, ref _currentView, value))
                UpdateView(value);
		}
	}

	#endregion

	#region TopSafeSpace

	public static readonly StyledProperty<double> TopSafeSpaceProperty =
		AvaloniaProperty.Register<NavigationBar, double>(nameof(TopSafeSpace));

	public double TopSafeSpace
	{
		get => GetValue(TopSafeSpaceProperty);
		set => SetValue(TopSafeSpaceProperty, value);
	}

	#endregion

	#region TopSafePadding

	public static readonly StyledProperty<Thickness> TopSafePaddingProperty =
		AvaloniaProperty.Register<NavigationBar, Thickness>(nameof(TopSafePadding));

	public Thickness TopSafePadding
	{
		get => GetValue(TopSafePaddingProperty);
		set => SetValue(TopSafePaddingProperty, value);
	}

	#endregion

	#region ApplyTopSafePadding

	public static readonly StyledProperty<bool> ApplyTopSafePaddingProperty =
		AvaloniaProperty.Register<NavigationBar, bool>(nameof(ApplyTopSafePadding), defaultValue: true);

	public bool ApplyTopSafePadding
	{
		get => GetValue(ApplyTopSafePaddingProperty);
		set => SetValue(ApplyTopSafePaddingProperty, value);
	}

	#endregion

	#region Attached properties

	#region Item

	public static readonly AttachedProperty<object> ItemProperty =
		AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, object>("Item");

	public static object GetItem(AvaloniaObject element) =>
		element.GetValue(ItemProperty);

	public static void SetItem(AvaloniaObject element, object parameter) =>
		element.SetValue(ItemProperty, parameter);

	#endregion

	#region Header

	public static readonly AttachedProperty<object> HeaderProperty =
		AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, object>("Header");

	public static object GetHeader(AvaloniaObject element) =>
		element.GetValue(HeaderProperty);

	public static void SetHeader(AvaloniaObject element, object parameter) =>
		element.SetValue(HeaderProperty, parameter);

	#endregion

	#region HeaderIcon

	public static readonly AttachedProperty<object> HeaderIconProperty =
		AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, object>("HeaderIcon");

	public static object GetHeaderIcon(AvaloniaObject element) =>
		element.GetValue(HeaderIconProperty);

	public static void SetHeaderIcon(AvaloniaObject element, object parameter) =>
		element.SetValue(HeaderIconProperty, parameter);

	#endregion

	#region Visible

	public static readonly AttachedProperty<bool> VisibleProperty =
		AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, bool>("Visible", defaultValue: true);

    public static bool GetVisible(AvaloniaObject element) =>
		element.GetValue(VisibleProperty);

	public static void SetVisible(AvaloniaObject element, bool parameter) =>
		element.SetValue(VisibleProperty, parameter);

	#endregion

	#endregion

	#region Setup and loading template

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ShellViewProperty)
		{
			ShellViewUpdated();
		}
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		if (_pendingHeader != null)
			UpdateView(_pendingHeader);
	}

	protected virtual void ShellViewUpdated()
	{
		if (ShellView is not { } shellView) return;

		_backCommand = shellView.BackCommand;
		_sideMenuCommand = shellView.SideMenuCommand;

		this[!TopSafePaddingProperty] = shellView[!ShellView.TopSafePaddingProperty];
		this[!TopSafeSpaceProperty] = shellView[!ShellView.TopSafeSpaceProperty];
		this[!ApplyTopSafePaddingProperty] = shellView[!ShellView.ApplyTopSafePaddingProperty];

		if (shellView.ContentView?.CurrentView is { } currentView)
			UpdateView(currentView);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_header = e.NameScope.Find<ContentControl>("PART_Header") ??
		          throw new Exception("PART_Header cannot found for NavigationBar");
		_actionButton = e.NameScope.Find<Button>("PART_ActionButton");
		_items = e.NameScope.Find<ContentControl>("PART_Items");

		if (_actionButton != null)
			_actionButton.Command = _backCommand;

		if (_pendingHeader != null)
			UpdateView(_pendingHeader);
	}

	#endregion

	#region Updates

	internal void UpdateView(object? view)
	{
		if (_header == null && _items == null)
		{
			_pendingHeader = view;
			return;
		}

		if (_header != null)
			UpdateHeader(view, _header);

		if (_items != null)
			UpdateItems(view, _items);

		if (view is StyledElement element)
			this[!IsVisibleProperty] = element[!VisibleProperty];
		else
			IsVisible = true;

		UpdateButtons();
		_pendingHeader = null;
	}

	protected virtual void UpdateButtons()
	{
		if (ShellView == null) return;

		var navService = ShellView?.Navigator;
		var hasItem = navService?.HasItemInStack() ?? false;

		if (_actionButton == null) return;

		_actionButton.Command = hasItem
			? BackCommand
			: SideMenuCommand;

		if (hasItem)
		{
			_actionButton.Classes.Remove("SideMenuButton");
			_actionButton.Classes.Add("BackButton");

			_actionButton.IsVisible = true;
		}
		else
		{
			_actionButton.Classes.Remove("BackButton");
			_actionButton.Classes.Add("SideMenuButton");

			_actionButton.IsVisible = HasSideMenuOption;
		}
	}

	protected virtual void UpdateItems(object? view, ContentControl itemPresenter)
	{
		if (view is not AvaloniaObject)
		{
			itemPresenter.Content = null;
			itemPresenter.DataContext = null;
			return;
		}

		itemPresenter.DataContext = null;
		if (view is StyledElement element)
		{
			itemPresenter[!ContentControl.ContentProperty] = element[!ItemProperty];

			if (itemPresenter.Content is StyledElement elementContent)
				elementContent.DataContext = element.DataContext ?? element;

			return;
		}

		itemPresenter.Content = view;
	}

	protected virtual void UpdateHeader(object? view, ContentControl itemPresenter)
	{
		if (view is not AvaloniaObject)
		{
			itemPresenter.Content = null;
			itemPresenter.DataContext = null;
			return;
		}

		if (view is StyledElement element)
		{
			itemPresenter.DataContext = element.DataContext ?? element;
			itemPresenter[!ContentControl.ContentProperty] = element[!HeaderProperty];
			return;
		}

		itemPresenter.Content = view;
	}

	#endregion
}
