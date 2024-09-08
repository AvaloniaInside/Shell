using System;
using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AvaloniaInside.Shell;

public class Page : UserControl, INavigationLifecycle, INavigatorLifecycle, INavigationBarProvider
{
	private ContentPresenter? _navigationBarPlaceHolder;
	private NavigationBar? _navigationBar;

	#region Properties

	public NavigationBar? NavigationBar => Shell?.NavigationBar;

	public NavigationBar? AttachedNavigationBar => _navigationBar;

	public INavigator? Navigator => Shell?.Navigator;

	public NavigationChain Chain { get; internal set; }

	protected override Type StyleKeyOverride => typeof(Page);

	#region Shell

	public static readonly StyledProperty<ShellView?> ShellProperty =
		AvaloniaProperty.Register<Page, ShellView?>(nameof(Shell));

	public ShellView? Shell
	{
		get => GetValue(ShellProperty);
		internal set => SetValue(ShellProperty, value);
	}

	#endregion

	#region SafePadding

	public static readonly StyledProperty<Thickness> SafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(SafePadding));

	public Thickness SafePadding
	{
		get => GetValue(SafePaddingProperty);
		set => SetValue(SafePaddingProperty, value);
	}

	#endregion

	#region ApplyTopSafePadding

	public static readonly StyledProperty<bool> ApplyTopSafePaddingProperty =
		AvaloniaProperty.Register<Page, bool>(nameof(ApplyTopSafePadding), defaultValue: true);

	public bool ApplyTopSafePadding
	{
		get => GetValue(ApplyTopSafePaddingProperty);
		set => SetValue(ApplyTopSafePaddingProperty, value);
	}

	#endregion

	#region ApplyBottomSafePadding

	public static readonly StyledProperty<bool> ApplyBottomSafePaddingProperty =
		AvaloniaProperty.Register<Page, bool>(nameof(ApplyBottomSafePadding), defaultValue: true);

	public bool ApplyBottomSafePadding
	{
		get => GetValue(ApplyBottomSafePaddingProperty);
		set => SetValue(ApplyBottomSafePaddingProperty, value);
	}

	#endregion

	#region ApplyLeftSafePadding

	public static readonly StyledProperty<bool> ApplyLeftSafePaddingProperty =
		AvaloniaProperty.Register<Page, bool>(nameof(ApplyLeftSafePadding), defaultValue: true);

	public bool ApplyLeftSafePadding
	{
		get => GetValue(ApplyLeftSafePaddingProperty);
		set => SetValue(ApplyLeftSafePaddingProperty, value);
	}

	#endregion

	#region ApplyRightSafePadding

	public static readonly StyledProperty<bool> ApplyRightSafePaddingProperty =
		AvaloniaProperty.Register<Page, bool>(nameof(ApplyRightSafePadding), defaultValue: true);

	public bool ApplyRightSafePadding
	{
		get => GetValue(ApplyRightSafePaddingProperty);
		set => SetValue(ApplyRightSafePaddingProperty, value);
	}

	#endregion

	#region TopSafeSpace

	public static readonly StyledProperty<double> TopSafeSpaceProperty =
		AvaloniaProperty.Register<Page, double>(nameof(TopSafeSpace));

	public double TopSafeSpace
	{
		get => GetValue(TopSafeSpaceProperty);
		set => SetValue(TopSafeSpaceProperty, value);
	}

	#endregion

	#region TopSafePadding

	public static readonly StyledProperty<Thickness> TopSafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(TopSafePadding));

	public Thickness TopSafePadding
	{
		get => GetValue(TopSafePaddingProperty);
		set => SetValue(TopSafePaddingProperty, value);
	}

	#endregion

	#region BottomSafeSpace

	public static readonly StyledProperty<double> BottomSafeSpaceProperty =
		AvaloniaProperty.Register<Page, double>(nameof(BottomSafeSpace));

	public double BottomSafeSpace
	{
		get => GetValue(BottomSafeSpaceProperty);
		set => SetValue(BottomSafeSpaceProperty, value);
	}

	#endregion

	#region BottomSafePadding

	public static readonly StyledProperty<Thickness> BottomSafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(BottomSafePadding));

	public Thickness BottomSafePadding
	{
		get => GetValue(BottomSafePaddingProperty);
		set => SetValue(BottomSafePaddingProperty, value);
	}

	#endregion

	#region LeftSafeSpace

	public static readonly StyledProperty<double> LeftSafeSpaceProperty =
		AvaloniaProperty.Register<Page, double>(nameof(LeftSafeSpace));

	public double LeftSafeSpace
	{
		get => GetValue(LeftSafeSpaceProperty);
		set => SetValue(LeftSafeSpaceProperty, value);
	}

	#endregion

	#region LeftSafePadding

	public static readonly StyledProperty<Thickness> LeftSafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(LeftSafePadding));

	public Thickness LeftSafePadding
	{
		get => GetValue(LeftSafePaddingProperty);
		set => SetValue(LeftSafePaddingProperty, value);
	}

	#endregion

	#region RightSafeSpace

	public static readonly StyledProperty<double> RightSafeSpaceProperty =
		AvaloniaProperty.Register<Page, double>(nameof(RightSafeSpace));

	public double RightSafeSpace
	{
		get => GetValue(RightSafeSpaceProperty);
		set => SetValue(RightSafeSpaceProperty, value);
	}

	#endregion

	#region RightSafePadding

	public static readonly StyledProperty<Thickness> RightSafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(RightSafePadding));

	public Thickness RightSafePadding
	{
		get => GetValue(RightSafePaddingProperty);
		set => SetValue(RightSafePaddingProperty, value);
	}

	#endregion

	#region PageSafePadding

	public static readonly StyledProperty<Thickness> PageSafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(PageSafePadding));

	public Thickness PageSafePadding
	{
		get => GetValue(PageSafePaddingProperty);
		set => SetValue(PageSafePaddingProperty, value);
	}

	#endregion

	#region TabSafePadding

	public static readonly StyledProperty<Thickness> TabSafePaddingProperty =
		AvaloniaProperty.Register<Page, Thickness>(nameof(TabSafePadding));

	public Thickness TabSafePadding
	{
		get => GetValue(TabSafePaddingProperty);
		set => SetValue(TabSafePaddingProperty, value);
	}

	#endregion

	#endregion

	#region Lifecycle

	public virtual Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task ArgumentAsync(object args, CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task TerminateAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public virtual Task OnNavigateAsync(NaviagateEventArgs args, CancellationToken cancellationToken) =>
		Task.CompletedTask;

	public virtual Task OnNavigatingAsync(NaviagatingEventArgs args, CancellationToken cancellationToken) =>
		Task.CompletedTask;

	#endregion

	#region Setup and template

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		ApplyNavigationBar();
		AttachedNavigationBar?.UpdateView(this);

		this[!ApplyTopSafePaddingProperty] = this[!ShellView.EnableSafeAreaForTopProperty];
		this[!ApplyBottomSafePaddingProperty] = this[!ShellView.EnableSafeAreaForBottomProperty];
		this[!ApplyLeftSafePaddingProperty] = this[!ShellView.EnableSafeAreaForLeftProperty];
		this[!ApplyRightSafePaddingProperty] = this[!ShellView.ApplyRightSafePaddingProperty];
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == ShellProperty)
		{
			if (Shell != null)
			{
				this[!SafePaddingProperty] = Shell[!ShellView.SafePaddingProperty];
			}
		}
		else if (change.Property == SafePaddingProperty)
		{
			UpdateSafePaddingSizes();
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_navigationBarPlaceHolder = e.NameScope.Find<ContentPresenter>("PART_NavigationBarPlaceHolder");
	}

	private void ApplyNavigationBar()
	{
		if (_navigationBarPlaceHolder == null || _navigationBar != null)
			return;

		if (Shell?.NavigationBarAttachType is not ({ } type and not NavigationBarAttachType.ToShell))
			return;

		if ((type == NavigationBarAttachType.ToLastPage && Chain is HostNavigationChain) ||
		    (type == NavigationBarAttachType.ToFirstHostThenPage && Chain?.Back is HostNavigationChain))
			return;

		_navigationBarPlaceHolder.Content = _navigationBar = new NavigationBar(this);
	}

	#endregion

	#region Sizing

	protected virtual void UpdateSafePaddingSizes()
	{
		TopSafeSpace = SafePadding.Top;
		TopSafePadding = new Thickness(0, SafePadding.Top, 0, 0);
		BottomSafeSpace = SafePadding.Bottom;
		BottomSafePadding = new Thickness(0, 0, 0, SafePadding.Bottom);
		LeftSafeSpace = SafePadding.Left;
		LeftSafePadding = new Thickness(SafePadding.Left, 0, 0, 0);
		RightSafeSpace = SafePadding.Right;
		RightSafePadding = new Thickness(0, 0, SafePadding.Right, 0);

		PageSafePadding  = new Thickness(SafePadding.Left, 0, SafePadding.Right, SafePadding.Bottom);

	}

	#endregion
}
