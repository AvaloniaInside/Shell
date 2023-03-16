using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using ReactiveUI;
using Splat;

namespace AvaloniaInside.Shell;

public partial class ShellView : TemplatedControl
{
	#region Enums

	public enum ScreenSizeType
	{
		Small,
		Medium,
		Large
	}

	public enum SideMenuBehaveType
	{
		Default,
		Keep,
		Closed
	}

	#endregion

	#region Variables

	private readonly bool _isMobile;

	private SplitView? _splitView;
	private StackContentView? _contentView;
	private NavigationBar? _navigationBar;
	private StackContentView? _modalView;
	private SideMenu? _sideMenu;

	#endregion

	#region Properties

	public NavigationBar NavigationBar => _navigationBar;

	#region ScreenSize

	private ScreenSizeType _screenSize = ScreenSizeType.Large;
	public static readonly DirectProperty<ShellView, ScreenSizeType> ScreenSizeProperty =
		AvaloniaProperty.RegisterDirect<ShellView, ScreenSizeType>(
			nameof(ScreenSize),
			o => o.ScreenSize,
			(o, v) => o.ScreenSize = v);
	public ScreenSizeType ScreenSize
	{
		get => _screenSize;
		set
		{
			var oldValue = _screenSize;
			if (SetAndRaise(ScreenSizeProperty, ref _screenSize, value))
				UpdateScreenSize(oldValue, value);
		}
	}

	#endregion

	#region DefaultRoute

	public static DirectProperty<ShellView, string?> DefaultRouteProperty = AvaloniaProperty
		.RegisterDirect<ShellView, string?>(
			nameof(DefaultRoute),
			o => o.DefaultRoute,
			(o, v) => o.DefaultRoute = v);

	public string? DefaultRoute { get; set; }

	#endregion

	#region NavigationTemplate

	public static readonly StyledProperty<IControlTemplate?> NavigationTemplateProperty =
		AvaloniaProperty.Register<ShellView, IControlTemplate?>(nameof(NavigationTemplate));

	public IControlTemplate? NavigationTemplate
	{
		get => GetValue(NavigationTemplateProperty);
		set => SetValue(NavigationTemplateProperty, value);
	}

	#endregion

	#endregion

	#region Ctor and loading

	public ShellView()
	{
		Navigator = Locator.Current
			.GetService<INavigator>() ?? throw new ArgumentException("Cannot find INavigationService");
		Navigator.RegisterShell(this);

		_isMobile = AvaloniaLocator.Current
			.GetService<IRuntimePlatform>()?
			.GetRuntimeInfo()
			.IsMobile ?? false;

		if (!_isMobile)
		{
			Classes.Add("Mobile");
			_sideMenuPresented = true;
		}

		SizeChanged += OnSizeChanged;
		Items.CollectionChanged += ItemsOnCollectionChanged;
	}

	protected override void OnLoaded()
	{
		base.OnLoaded();

		if (TopLevel.GetTopLevel(this) is { } topLevel)
		{
			topLevel.BackRequested += TopLevelOnBackRequested;
			topLevel.KeyUp += TopLevelOnKeyUp;
		}

		if (DefaultRoute != null)
		{
			_ = Navigator.NavigateAsync(DefaultRoute, CancellationToken.None);
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_splitView = e.NameScope.Find<SplitView>("PART_SplitView");
		_contentView = e.NameScope.Find<StackContentView>("PART_ContentView");
		_modalView = e.NameScope.Find<StackContentView>("PART_Modal");
		_navigationBar = e.NameScope.Find<NavigationBar>("PART_NavigationBar");
		_sideMenu = e.NameScope.Find<SideMenu>("PART_SideMenu");

		SetupUi();

		if (_splitView != null)
		{
			_splitView.PaneClosing += SplitViewOnPaneClosing;
		}

		if (_sideMenu != null)
		{
			_sideMenu.Items = _sideMenuItems;
		}
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		switch (change.Property.Name)
		{
			case nameof(LargeScreenSideMenuMode):
			case nameof(MediumScreenSideMenuMode):
			case nameof(SmallScreenSideMenuMode):
				UpdateSideMenu();
				break;
		}
	}

	private void SetupUi()
	{
		if (_navigationBar != null)
		{
			_navigationBar.ShellView = this;
			_navigationBar.BackCommand = ReactiveCommand.CreateFromTask(BackActionAsync);
			_navigationBar.SideMenuCommand = ReactiveCommand.CreateFromTask(MenuActionAsync);
		}
	}

	#endregion

	#region Services and navigation

	public INavigator Navigator { get; }

	#endregion

	#region View Stack Manager

	public async Task PushViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await (_contentView?.PushViewAsync(view, cancellationToken) ?? Task.CompletedTask);
		SelectSideMenuItem();
	}

	public async Task RemoveViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await (_contentView?.RemoveViewAsync(view, cancellationToken) ?? Task.CompletedTask);
		await (_modalView?.RemoveViewAsync(view, cancellationToken) ?? Task.CompletedTask);
	}

	public async Task ClearStackAsync(CancellationToken cancellationToken)
	{
		await (_contentView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
		await (_modalView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
	}

	public Task ModalAsync(object instance, CancellationToken cancellationToken) =>
		_modalView?.PushViewAsync(instance, cancellationToken) ?? Task.CompletedTask;

	private bool Back()
	{
		if (ScreenSize == ScreenSizeType.Small && SideMenuPresented)
		{
			SideMenuPresented = false;
			return true;
		}

		var result = Navigator.HasItemInStack();
		if (result)
			Navigator.BackAsync();

		return result;
	}

	#endregion

	#region Ui Events

	private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
	{
		ScreenSize = e.NewSize.Width switch
		{
			<= 768 => ScreenSizeType.Small,
			<= 1024 => ScreenSizeType.Medium,
			_ => ScreenSizeType.Large,
		};
	}

	private void TopLevelOnBackRequested(object? sender, RoutedEventArgs e)
	{
		e.Handled = Back();
	}

	private void TopLevelOnKeyUp(object? sender, KeyEventArgs e)
	{
		if (e.Key == Key.Escape)
			Back();
	}

	private void SplitViewOnPaneClosing(object? sender, CancelRoutedEventArgs e)
	{
		SideMenuPresented = false;
	}

	protected virtual Task BackActionAsync(CancellationToken cancellationToken)
	{
		return Navigator.BackAsync(cancellationToken);
	}

	#endregion

	#region Screen Actions

	private void UpdateScreenSize(ScreenSizeType old, ScreenSizeType newScreen)
	{
		Classes.Add(newScreen.ToString());
		Classes.Remove(old.ToString());

		if (_splitView == null) return;

		_splitView.DisplayMode = newScreen switch
		{
			ScreenSizeType.Small => SmallScreenSideMenuMode,
			ScreenSizeType.Medium => MediumScreenSideMenuMode,
			ScreenSizeType.Large => LargeScreenSideMenuMode,
			_ => throw new ArgumentOutOfRangeException(nameof(newScreen), newScreen, null)
		};

		if (newScreen == ScreenSizeType.Small && SideMenuPresented)
			SideMenuPresented = false;
		else
			UpdateSideMenu();
	}

	#endregion
}
