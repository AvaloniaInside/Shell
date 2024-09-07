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

		_navigationBarPlaceHolder.Content = _navigationBar = new NavigationBar
		{
			ShellView = Shell
		};
	}

	#endregion
}
