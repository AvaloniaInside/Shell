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

	public NavigationBar? NavigationBar => Shell?.NavigationBar;

	public NavigationBar? AttachedNavigationBar => _navigationBar;

	public ShellView? Shell
	{
		get => GetValue(ShellProperty);
		set => SetValue(ShellProperty, value);
	}

	public INavigator? Navigator => Shell?.Navigator;

	public NavigationChain Chain { get; internal set; }

	protected override Type StyleKeyOverride => typeof(Page);

	public virtual Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task ArgumentAsync(object args, CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	public virtual Task TerminateAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public virtual Task OnNavigateAsync(NaviagateEventArgs args, CancellationToken cancellationToken) =>
		Task.CompletedTask;

	public virtual Task OnNavigatingAsync(NaviagatingEventArgs args, CancellationToken cancellationToken) =>
		Task.CompletedTask;

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_navigationBarPlaceHolder = e.NameScope.Find<ContentPresenter>("PART_NavigationBarPlaceHolder");
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);
		ApplyNavigationBar();
		AttachedNavigationBar?.UpdateView(this);
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

		_navigationBarPlaceHolder.Content = _navigationBar = new NavigationBar
		{
			ShellView = Shell
		};
	}
}
