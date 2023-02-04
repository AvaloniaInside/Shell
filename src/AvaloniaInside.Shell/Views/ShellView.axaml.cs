using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using AvaloniaInside.Shell.Services;

namespace AvaloniaInside.Shell.Views;

public partial class ShellView : TemplatedControl
{
	private StackContentView _contentView;
	private NavigationView _navigationView;

	public static readonly DirectProperty<ShellView, object?> DefaultContentProperty =
		AvaloniaProperty.RegisterDirect<ShellView, object?>(
			nameof(DefaultContent),
			o => o.DefaultContent,
			(o, v) => o.DefaultContent = v);

	private object? _defaultContent;

	[Content]
	public object? DefaultContent
	{
		get => _defaultContent;
		set
		{
			if (_defaultContent != null)
				throw new InvalidOperationException("Default content can be set only one time");

			_defaultContent = value;
		}
	}

	public ShellView()
	{
		InitializeComponent();

		var navigationService = AvaloniaLocator.Current.GetService<INavigationService>() ??
		                        throw new ArgumentNullException(nameof(INavigationService),
			                        "Cannot find INavigationService");

		navigationService.Navigate += NavigationServiceOnNavigate;
	}

	private void NavigationServiceOnNavigate(object sender, NavigateEventArgs e)
	{
		var view = AvaloniaLocator.Current
			.GetService<INavigationViewLocator>()
			.GetView(e.Node);

		if (view == _contentView.CurrentView)
		{
			Debug.WriteLine("Warning: Navigate to current view, skipped");
			return;
		}

		if (_contentView.IsExistsInStack(view))
		{
			//_contentView.
		}
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_contentView = e.NameScope.Find<StackContentView>("PART_ContentView");
		_navigationView = e.NameScope.Find<NavigationView>("PART_NavigationView");

		if (_defaultContent != null)
			_ = PushViewAsync(_defaultContent);
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public async Task PushViewAsync(object view, CancellationToken cancellationToken = default)
	{
		await _contentView.PushViewAsync(view, cancellationToken);
		if (view is INavigation navigation)
		{
			await _navigationView.PushNavigationStage(navigation, cancellationToken);
		}
	}
}
