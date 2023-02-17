using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace AvaloniaInside.Shell;

public class NavigationView : StackContentView
{
	private ContentPresenter? _header;
	private Button? _backButton;
	private object? _pendingHeader;

	private readonly ICommand _backCommand;

	public NavigationView()
	{
		_backCommand = ReactiveCommand.CreateFromTask(BackActionAsync);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_header = e.NameScope.Find<ContentPresenter>("PART_Header") ?? throw new ArgumentNullException("PART_Header");
		_backButton = e.NameScope.Find<Button>("PART_BackButton");

		if (_backButton != null)
			_backButton.Command = _backCommand;

		if (_pendingHeader != null)
			_ = UpdateAsync(_pendingHeader, CancellationToken.None);
	}

	protected override Task OnContentUpdateAsync(object? view, CancellationToken cancellationToken)
	{
		if (_header != null && view != null)
			return UpdateAsync(view, cancellationToken);

		_pendingHeader = view;
		return Task.CompletedTask;
	}

	private Task UpdateAsync(object view, CancellationToken cancellationToken)
	{
		if (_header != null)
			_header.Content = GetTitle(view);

		if (view is INavigation navigation)
		{
			//TODO: implement actions
		}

		UpdateButtons();
		return Task.CompletedTask;
	}

	private object? GetTitle(object view) =>
		view switch
		{
			INavigation navigation => navigation.Header,
			Window window => window.Title,
			Control control => control.Name,
			_ => view.GetType().Name
		};

	protected virtual void UpdateButtons()
	{
		var navService = AvaloniaLocator.CurrentMutable.GetService<INavigationService>();
		var hasItem = navService?.HasItemInStack() ?? false;

		if (_backButton != null)
			_backButton.IsVisible = hasItem;
	}

	protected virtual Task BackActionAsync(CancellationToken cancellationToken)
	{
		return AvaloniaLocator.CurrentMutable
			.GetService<INavigationService>()?
			.BackAsync(cancellationToken) ?? Task.CompletedTask;
	}
}

