using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class ShellView : TemplatedControl
{
	private StackContentView? _contentView;
	private NavigationView? _navigationView;
	private StackContentView? _modalView;

	public static ShellView? Current { get; private set; }

	public static DirectProperty<ShellView, string?> DefaultRouteProperty = AvaloniaProperty
		.RegisterDirect<ShellView, string?>(
			nameof(DefaultRoute),
			o => o.DefaultRoute,
			(o, v) => o.DefaultRoute = v);

	public string? DefaultRoute { get; set; }

	public ShellView()
	{
		Current = this;
	}

	protected override void OnLoaded()
	{
		Current = this;
		base.OnLoaded();
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_contentView = e.NameScope.Find<StackContentView>("PART_ContentView");
		_modalView = e.NameScope.Find<StackContentView>("PART_Modal");
		_navigationView = e.NameScope.Find<NavigationView>("PART_NavigationView");

		if (DefaultRoute != null)
		{
			_ = AvaloniaLocator.CurrentMutable.GetService<INavigationService>()?
				.NavigateAsync(DefaultRoute, CancellationToken.None);
		}
	}

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
}
