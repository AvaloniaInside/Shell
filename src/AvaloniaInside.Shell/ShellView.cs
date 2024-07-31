using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaInside.Shell.Platform;
using AvaloniaInside.Shell.Platform.Windows;
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
        Closed,
        Removed
    }

    #endregion

    #region Variables

    private readonly bool _isMobile;

    private SplitView? _splitView;
    private StackContentView? _contentView;
    private NavigationBar? _navigationBar;
    private StackContentView? _modalView;
    private SideMenu? _sideMenu;

    private bool _loadedFlag = false;
    private bool _topLevelEventFlag = false;

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

    #region TopSafeSpace

    public static readonly StyledProperty<double> TopSafeSpaceProperty =
        AvaloniaProperty.Register<ShellView, double>(nameof(TopSafeSpace));

    public double TopSafeSpace
    {
        get => GetValue(TopSafeSpaceProperty);
        set => SetValue(TopSafeSpaceProperty, value);
    }

    #endregion

    #region BottomSafeSpace

    public static readonly StyledProperty<double> BottomSafeSpaceProperty =
        AvaloniaProperty.Register<ShellView, double>(nameof(BottomSafeSpace));

    public double BottomSafeSpace
    {
        get => GetValue(BottomSafeSpaceProperty);
        set => SetValue(BottomSafeSpaceProperty, value);
    }

    #endregion

    #region TopSafePadding

    public static readonly StyledProperty<Thickness> TopSafePaddingProperty =
        AvaloniaProperty.Register<ShellView, Thickness>(nameof(TopSafePadding));

    public Thickness TopSafePadding
    {
        get => GetValue(TopSafePaddingProperty);
        set => SetValue(TopSafePaddingProperty, value);
    }

    #endregion

    #region BottomSafePadding

    public static readonly StyledProperty<Thickness> BottomSafePaddingProperty =
        AvaloniaProperty.Register<ShellView, Thickness>(nameof(BottomSafePadding));

    public Thickness BottomSafePadding
    {
        get => GetValue(BottomSafePaddingProperty);
        set => SetValue(BottomSafePaddingProperty, value);
    }

    #endregion

    #region SafePadding

    public static readonly StyledProperty<Thickness> SafePaddingProperty =
        AvaloniaProperty.Register<ShellView, Thickness>(nameof(SafePadding));

    public Thickness SafePadding
    {
        get => GetValue(SafePaddingProperty);
        set => SetValue(SafePaddingProperty, value);
    }

    #endregion

    #region ApplyTopSafePadding

    public static readonly StyledProperty<bool> ApplyTopSafePaddingProperty =
        AvaloniaProperty.Register<ShellView, bool>(nameof(ApplyTopSafePadding), defaultValue: true);

    public bool ApplyTopSafePadding
    {
        get => GetValue(ApplyTopSafePaddingProperty);
        set => SetValue(ApplyTopSafePaddingProperty, value);
    }

    #endregion

    #region ApplyBottomSafePadding

    public static readonly StyledProperty<bool> ApplyBottomSafePaddingProperty =
        AvaloniaProperty.Register<ShellView, bool>(nameof(ApplyBottomSafePadding), defaultValue: true);

    public bool ApplyBottomSafePadding
    {
        get => GetValue(ApplyBottomSafePaddingProperty);
        set => SetValue(ApplyBottomSafePaddingProperty, value);
    }

    #endregion

    #region DefaultPageTransition

    /// <summary>
    /// Defines the <see cref="DefaultPageTransitionProperty"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> DefaultPageTransitionProperty =
        AvaloniaProperty.Register<ShellView, IPageTransition?>(
            nameof(DefaultPageTransition),
            defaultValue: PlatformSetup.TransitionForPage);

    /// <summary>
    /// Gets or sets the animation played when content appears and disappears.
    /// </summary>
    public IPageTransition? DefaultPageTransition
    {
        get => GetValue(DefaultPageTransitionProperty);
        set => SetValue(DefaultPageTransitionProperty, value);
    }

    #endregion

    #region ModalPageTransition

    /// <summary>
    /// Defines the <see cref="DefaultPageTransitionProperty"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> ModalPageTransitionProperty =
	    AvaloniaProperty.Register<ShellView, IPageTransition?>(
		    nameof(ModalPageTransition),
		    defaultValue: PlatformSetup.TransitionForModal);

    /// <summary>
    /// Gets or sets the animation played when content appears and disappears.
    /// </summary>
    public IPageTransition? ModalPageTransition
    {
	    get => GetValue(ModalPageTransitionProperty);
	    set => SetValue(ModalPageTransitionProperty, value);
    }

    #endregion

    #endregion

    #region Attached properties

    #region EnableSafeAreaForTop

    public static readonly AttachedProperty<bool> EnableSafeAreaForTopProperty =
        AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, bool>("EnableSafeAreaForTop",
            defaultValue: true);

    public static bool GetEnableSafeAreaForTop(AvaloniaObject element) =>
        element.GetValue(EnableSafeAreaForTopProperty);

    public static void SetEnableSafeAreaForTop(AvaloniaObject element, bool parameter) =>
        element.SetValue(EnableSafeAreaForTopProperty, parameter);

    #endregion

    #region EnableSafeAreaForBottom

    public static readonly AttachedProperty<bool> EnableSafeAreaForBottomProperty =
        AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, bool>("EnableSafeAreaForBottom",
            defaultValue: true);

    public static bool GetEnableSafeAreaForBottom(AvaloniaObject element) =>
        element.GetValue(EnableSafeAreaForBottomProperty);

    public static void SetEnableSafeAreaForBottom(AvaloniaObject element, bool parameter) =>
        element.SetValue(EnableSafeAreaForBottomProperty, parameter);

    #endregion

    #endregion

    #region Ctor and loading

    public ShellView()
    {
        Navigator = Locator.Current
            .GetService<INavigator>() ?? throw new ArgumentException("Cannot find INavigationService");
        Navigator.RegisterShell(this);

        var isMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
        if (!_isMobile)
        {
            Classes.Add("Mobile");
            _sideMenuPresented = true;
        }

        SizeChanged += OnSizeChanged;
        Items.CollectionChanged += ItemsOnCollectionChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (TopLevel.GetTopLevel(this) is { } topLevel && !_topLevelEventFlag)
        {
            topLevel.BackRequested += TopLevelOnBackRequested;
            topLevel.KeyUp += TopLevelOnKeyUp;
            _topLevelEventFlag = true;
        }

        if (DefaultRoute != null  && !_loadedFlag)
        {
            _ = Navigator.NavigateAsync(DefaultRoute, CancellationToken.None);
            _loadedFlag = true;
        }

        if (TopLevel.GetTopLevel(this) is { InsetsManager: { } insetsManager })
        {
            insetsManager.DisplayEdgeToEdge = true;
        }

        OnSafeEdgeSetup();
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
            case nameof(SafePadding):
                OnSafeEdgeSetup();
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

        OnSafeEdgeSetup();
        UpdateSideMenu();
    }

    protected virtual void OnSafeEdgeSetup()
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await Task.Delay(100);

            if (TopLevel.GetTopLevel(this) is { InsetsManager: { DisplayEdgeToEdge: true } insetsManager })
                SafePadding = insetsManager.SafeAreaPadding;

            TopSafeSpace = SafePadding.Top;
            TopSafePadding = new Thickness(0, SafePadding.Top, 0, 0);
            BottomSafeSpace = SafePadding.Bottom;
            BottomSafePadding = new Thickness(0, 0, 0, SafePadding.Bottom);
        });
    }

    #endregion

    #region Services and navigation

    public INavigator Navigator { get; }

    #endregion

    #region View Stack Manager

    public async Task PushViewAsync(object view,
        NavigateType navigateType,
        CancellationToken cancellationToken = default)
    {
        await (_contentView?.PushViewAsync(view, navigateType, cancellationToken) ?? Task.CompletedTask);
        SelectSideMenuItem();
        UpdateBindings();
    }

    public async Task RemoveViewAsync(object view,
        NavigateType navigateType,
        CancellationToken cancellationToken = default)
    {
        await (_contentView?.RemoveViewAsync(view, navigateType, cancellationToken) ?? Task.CompletedTask);
        await (_modalView?.RemoveViewAsync(view, navigateType, cancellationToken) ?? Task.CompletedTask);
    }

    public async Task ClearStackAsync(CancellationToken cancellationToken)
    {
        await (_contentView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
        await (_modalView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
    }

    public Task ModalAsync(object instance, NavigateType navigateType, CancellationToken cancellationToken) =>
        _modalView?.PushViewAsync(instance, navigateType, cancellationToken) ?? Task.CompletedTask;

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

    private void UpdateBindings()
    {
        var view = _contentView.CurrentView;
        if (view is StyledElement element)
        {
            this[!ApplyTopSafePaddingProperty] = element[!EnableSafeAreaForTopProperty];
            this[!ApplyBottomSafePaddingProperty] = element[!EnableSafeAreaForBottomProperty];
        }
        else
        {
            ApplyTopSafePadding = true;
            ApplyBottomSafePadding = true;
        }
    }

    #endregion
}
