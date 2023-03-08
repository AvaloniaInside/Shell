using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using ReactiveUI;

namespace AvaloniaInside.Shell;

public class ShellView : TemplatedControl
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

	public const double DefaultSideMenuSize = 250;

	private readonly bool _isMobile;

	private SplitView? _splitView;
	private StackContentView? _contentView;
	private NavigationView? _navigationView;
	private StackContentView? _modalView;

	#endregion

	#region Properties

	public static ShellView? Current { get; private set; }

	public NavigationView NavigationView => _navigationView;

	public double SideMenuSize => ScreenSize == ScreenSizeType.Small ? DesiredSize.Width - 35 : DefaultSideMenuSize;

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

	#region SideMenuPresented

	private bool _sideMenuPresented;
	public static readonly DirectProperty<ShellView, bool> SideMenuPresentedProperty =
		AvaloniaProperty.RegisterDirect<ShellView, bool>(
			nameof(SideMenuPresented),
			o => o.SideMenuPresented,
			(o, v) => o.SideMenuPresented = v);
	public bool SideMenuPresented
	{
		get => _sideMenuPresented;
		set
		{
			if (SetAndRaise(SideMenuPresentedProperty, ref _sideMenuPresented, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region LargeScreenSideMenuBehave

	private SideMenuBehaveType _largeScreenSideMenuBehave = SideMenuBehaveType.Keep;
	public static readonly DirectProperty<ShellView, SideMenuBehaveType> LargeScreenSideMenuBehaveProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuBehaveType>(
			nameof(SideMenuBehaveType),
			o => o.LargeScreenSideMenuBehave,
			(o, v) => o.LargeScreenSideMenuBehave = v);
	public SideMenuBehaveType LargeScreenSideMenuBehave
	{
		get => _largeScreenSideMenuBehave;
		set
		{
			if (SetAndRaise(LargeScreenSideMenuBehaveProperty, ref _largeScreenSideMenuBehave, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region MediumScreenSideMenuBehave

	private SideMenuBehaveType _mediumScreenSideMenuBehave = SideMenuBehaveType.Default;
	public static readonly DirectProperty<ShellView, SideMenuBehaveType> MediumScreenSideMenuBehaveProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuBehaveType>(
			nameof(SideMenuBehaveType),
			o => o.MediumScreenSideMenuBehave,
			(o, v) => o.MediumScreenSideMenuBehave = v);
	public SideMenuBehaveType MediumScreenSideMenuBehave
	{
		get => _mediumScreenSideMenuBehave;
		set
		{
			if (SetAndRaise(MediumScreenSideMenuBehaveProperty, ref _mediumScreenSideMenuBehave, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region SmallScreenSideMenuBehave

	private SideMenuBehaveType _smallScreenSideMenuBehave = SideMenuBehaveType.Default;
	public static readonly DirectProperty<ShellView, SideMenuBehaveType> SmallScreenSideMenuBehaveProperty =
		AvaloniaProperty.RegisterDirect<ShellView, SideMenuBehaveType>(
			nameof(SideMenuBehaveType),
			o => o.SmallScreenSideMenuBehave,
			(o, v) => o.SmallScreenSideMenuBehave = v);
	public SideMenuBehaveType SmallScreenSideMenuBehave
	{
		get => _smallScreenSideMenuBehave;
		set
		{
			if (SetAndRaise(SmallScreenSideMenuBehaveProperty, ref _smallScreenSideMenuBehave, value))
				UpdateSideMenu();
		}
	}

	#endregion

	#region LargeScreenSideMenuMode

	public static readonly StyledProperty<SplitViewDisplayMode> LargeScreenSideMenuModeProperty =
		AvaloniaProperty.Register<ShellView, SplitViewDisplayMode>(
			nameof(LargeScreenSideMenuMode),
			defaultValue: SplitViewDisplayMode.Inline,
			notifying: (o, b) => ((ShellView)o).UpdateSideMenu());

	public SplitViewDisplayMode LargeScreenSideMenuMode
	{
		get => GetValue(LargeScreenSideMenuModeProperty);
		set => SetValue(LargeScreenSideMenuModeProperty, value);
	}

	#endregion

	#region MediumScreenSideMenuMode

	public static readonly StyledProperty<SplitViewDisplayMode> MediumScreenSideMenuModeProperty =
		AvaloniaProperty.Register<ShellView, SplitViewDisplayMode>(
			nameof(MediumScreenSideMenuMode),
			defaultValue: SplitViewDisplayMode.CompactInline,
			notifying: (o, b) => ((ShellView)o).UpdateSideMenu());

	public SplitViewDisplayMode MediumScreenSideMenuMode
	{
		get => GetValue(MediumScreenSideMenuModeProperty);
		set => SetValue(MediumScreenSideMenuModeProperty, value);
	}

	#endregion

	#region SmallScreenSideMenuMode

	public static readonly StyledProperty<SplitViewDisplayMode> SmallScreenSideMenuModeProperty =
		AvaloniaProperty.Register<ShellView, SplitViewDisplayMode>(
			nameof(SmallScreenSideMenuMode),
			defaultValue: SplitViewDisplayMode.Overlay,
			notifying: (o, b) => ((ShellView)o).UpdateSideMenu());

	public SplitViewDisplayMode SmallScreenSideMenuMode
	{
		get => GetValue(SmallScreenSideMenuModeProperty);
		set => SetValue(SmallScreenSideMenuModeProperty, value);
	}

	#endregion

	#endregion

	#region Ctor and loading

	public ShellView()
	{
		Current = this;
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
	}

	protected override void OnLoaded()
	{
		Current = this;
		base.OnLoaded();

		if (TopLevel.GetTopLevel(this) is { } topLevel)
		{
			topLevel.BackRequested += TopLevelOnBackRequested;
			topLevel.KeyUp += TopLevelOnKeyUp;
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_splitView = e.NameScope.Find<SplitView>("PART_SplitView");
		_contentView = e.NameScope.Find<StackContentView>("PART_ContentView");
		_modalView = e.NameScope.Find<StackContentView>("PART_Modal");
		_navigationView = e.NameScope.Find<NavigationView>("PART_NavigationView");

		SetupUi();

		if (DefaultRoute != null)
		{
			_ = AvaloniaLocator.CurrentMutable.GetService<INavigationService>()?
				.NavigateAsync(DefaultRoute, CancellationToken.None);
		}

		if (_splitView != null)
		{
			_splitView.PaneClosing += SplitViewOnPaneClosing;
		}
	}

	private void SetupUi()
	{
		if (_navigationView != null)
		{
			_navigationView.BackCommand = ReactiveCommand.CreateFromTask(BackActionAsync);
			_navigationView.SideMenuCommand = ReactiveCommand.CreateFromTask(MenuActionAsync);
		}
	}

	#endregion

	#region View Stack Manager

	public async Task PushViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await (_contentView?.PushViewAsync(view, cancellationToken) ?? Task.CompletedTask);
		await (_navigationView?.PushViewAsync(view, cancellationToken) ?? Task.CompletedTask);
	}

	public async Task RemoveViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await (_contentView?.RemoveViewAsync(view, cancellationToken) ?? Task.CompletedTask);
		await (_navigationView?.RemoveViewAsync(view, cancellationToken) ?? Task.CompletedTask);
		await (_modalView?.RemoveViewAsync(view, cancellationToken) ?? Task.CompletedTask);
	}

	public async Task ClearStackAsync(CancellationToken cancellationToken)
	{
		await (_contentView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
		await (_navigationView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
		await (_modalView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
	}

	public Task ModalAsync(object instance, CancellationToken cancellationToken) =>
		_modalView?.PushViewAsync(instance, cancellationToken) ?? Task.CompletedTask;


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

	private void SplitViewOnPaneClosing(object? sender, SplitViewPaneClosingEventArgs e)
	{
		SideMenuPresented = false;
	}

	#endregion

	protected virtual Task BackActionAsync(CancellationToken cancellationToken)
	{
		return AvaloniaLocator.CurrentMutable
			.GetService<INavigationService>()?
			.BackAsync(cancellationToken) ?? Task.CompletedTask;
	}

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

	protected virtual Task MenuActionAsync(CancellationToken cancellationToken)
	{
		SideMenuPresented = !SideMenuPresented;
		return Task.CompletedTask;
	}

	protected virtual void UpdateSideMenu()
	{
		if (_splitView == null || _navigationView == null) return;

		var behave = ScreenSize switch
		{
			ScreenSizeType.Small => SmallScreenSideMenuBehave,
			ScreenSizeType.Medium => MediumScreenSideMenuBehave,
			ScreenSizeType.Large => LargeScreenSideMenuBehave,
			_ => throw new ArgumentOutOfRangeException()
		};

		if (behave == SideMenuBehaveType.Default)
		{
			_splitView.OpenPaneLength = SideMenuPresented ? SideMenuSize : 0;
			_splitView.IsPaneOpen = SideMenuPresented;
			_navigationView.HasSideMenuOption = true;
		}
		else if (behave == SideMenuBehaveType.Keep)
		{
			_splitView.OpenPaneLength = SideMenuSize;
			_splitView.IsPaneOpen = true;
			_navigationView.HasSideMenuOption = false;
		}
		else if (behave == SideMenuBehaveType.Closed)
		{
			_splitView.OpenPaneLength = 0;
			_splitView.IsPaneOpen = true;
			_navigationView.HasSideMenuOption = true;
		}
	}

	private bool Back()
	{
		if (ScreenSize == ScreenSizeType.Small && SideMenuPresented)
		{
			SideMenuPresented = false;
			return true;
		}

		if (AvaloniaLocator.CurrentMutable
			    .GetService<INavigationService>() is not {} navService) return false;

		var result = navService.HasItemInStack();
		if (result)
			navService.BackAsync();

		return result;
	}

}
