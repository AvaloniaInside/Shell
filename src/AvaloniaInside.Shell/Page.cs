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
	public static readonly StyledProperty<ShellView?> ShellProperty =
		AvaloniaProperty.Register<Page, ShellView?>(nameof(Shell));

	private ContentPresenter? _navigationBarPlaceHolder;
	private NavigationBar? _navigationBar;

	public NavigationBar? NavigationBar => FindNavigationBar(true) ?? Shell?.AttachedNavigationBar;

	public NavigationBar? AttachedNavigationBar => _navigationBar;

	public ShellView? Shell
	{
		get => GetValue(ShellProperty);
		set => SetValue(ShellProperty, value);
	}

    public INavigator? Navigator => Shell?.Navigator;

    protected override Type StyleKeyOverride => typeof(Page);

    public virtual Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task ArgumentAsync(object args, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task TerminateAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task OnNavigateAsync(NaviagateEventArgs args, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task OnNavigatingAsync(NaviagatingEventArgs args, CancellationToken cancellationToken) => Task.CompletedTask;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
	    base.OnApplyTemplate(e);
	    _navigationBarPlaceHolder = e.NameScope.Find<ContentPresenter>("PART_NavigationBarPlaceHolder");
	    _navigationBar = e.NameScope.Find<NavigationBar>("PART_NavigationBar");
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
	    base.OnLoaded(e);

	    if (_navigationBarPlaceHolder == null) return;
	    if (FindNavigationBar(true) != null) return;

	    _navigationBarPlaceHolder.Content = _navigationBar = new NavigationBar()
	    {
		    ShellView = Shell
	    };
    }

    private NavigationBar? FindNavigationBar(bool includeSelf)
    {
	    if (includeSelf && _navigationBar != null) return _navigationBar;
	    if (Shell?.AttachedNavigationBar is { } shellAttachedNavigationBar) return shellAttachedNavigationBar;
	    if (Navigator?.CurrentChain?.GetAscendingNodes() is not { } nodes) return null;

	    foreach (var item in nodes)
	    {
		    if (!item.Hosted) return null;
		    if (item.Instance is INavigationBarProvider { AttachedNavigationBar: { } navigationBar })
			    return navigationBar;
	    }

	    return null;
    }
}
