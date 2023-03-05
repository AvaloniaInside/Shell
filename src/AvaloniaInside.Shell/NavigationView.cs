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
	private Button? _actionButton;
	private ContentPresenter? _itemsContentPresenter;

	private object? _pendingHeader;

	private readonly ICommand _backCommand;
	private readonly ICommand _flyoutCommand;

	public NavigationView()
	{
		_backCommand = ReactiveCommand.CreateFromTask(BackActionAsync);
		_flyoutCommand = ReactiveCommand.CreateFromTask(FlyoutActionAsync);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_header = e.NameScope.Find<ContentPresenter>("PART_Header") ?? throw new ArgumentNullException("PART_Header");
		_actionButton = e.NameScope.Find<Button>("PART_ActionButton");
		_itemsContentPresenter = e.NameScope.Find<ContentPresenter>("PART_Items");

		if (_actionButton != null)
			_actionButton.Command = _backCommand;

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

	public Task UpdateAsync(object view, CancellationToken cancellationToken)
	{
		if (_header == null && _itemsContentPresenter == null)
		{
			_pendingHeader = view;
			return Task.CompletedTask;
		}

		if (_header != null)
			_header.Content = GetTitle(view);

		if (_itemsContentPresenter != null)
			UpdateItems(view, _itemsContentPresenter);

		UpdateButtons();
		return Task.CompletedTask;
	}

	private object? GetTitle(object view) =>
		view switch
		{
			INavigation navigation => navigation.Title,
			Window window => window.Title,
			Control control => control.Name,
			_ => view.GetType().Name
		};

	protected virtual void UpdateButtons()
	{
		var navService = AvaloniaLocator.CurrentMutable.GetService<INavigationService>();
		var hasItem = navService?.HasItemInStack() ?? false;

		if (_actionButton == null) return;

		_actionButton.Command = hasItem
			? _backCommand
			: _flyoutCommand;

		if (hasItem)
		{
			_actionButton.Classes.Remove("FlyoutButton");
			_actionButton.Classes.Add("BackButton");
		}
		else
		{
			_actionButton.Classes.Remove("BackButton");
			_actionButton.Classes.Add("FlyoutButton");
		}
	}

	protected virtual void UpdateItems(object view, ContentPresenter itemPresenter)
	{
		itemPresenter.Content = view is INavigation navigation ? navigation.Item : null;
	}

	protected virtual Task BackActionAsync(CancellationToken cancellationToken)
	{
		return AvaloniaLocator.CurrentMutable
			.GetService<INavigationService>()?
			.BackAsync(cancellationToken) ?? Task.CompletedTask;
	}

	private Task FlyoutActionAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
