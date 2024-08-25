using System;
using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class Page : UserControl, INavigationLifecycle, INavigatorLifecycle
{
	public static readonly StyledProperty<ShellView?> ShellProperty =
		AvaloniaProperty.Register<Page, ShellView?>(nameof(Shell));

	private NavigationBar? _navigationBar;

	public NavigationBar? NavigationBar => _navigationBar ?? Shell?.AttachedNavigationBar;

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
	    _navigationBar = e.NameScope.Find<NavigationBar>("PART_NavigationBar");
    }
}
