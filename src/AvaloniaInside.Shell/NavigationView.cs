using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class NavigationView : StackContentView
{
	public static readonly DirectProperty<NavigationView, ICommand> SideMenuCommandProperty =
		AvaloniaProperty.RegisterDirect<NavigationView, ICommand>(
			nameof(SideMenuCommand),
			o => o.SideMenuCommand,
			(o, v) => o.SideMenuCommand = v);

	public static readonly DirectProperty<NavigationView, ICommand> BackCommandProperty =
		AvaloniaProperty.RegisterDirect<NavigationView, ICommand>(
			nameof(BackCommand),
			o => o.BackCommand,
			(o, v) => o.BackCommand = v);

	public static readonly DirectProperty<NavigationView, bool> HasSideMenuOptionProperty =
		AvaloniaProperty.RegisterDirect<NavigationView, bool>(
			nameof(HasSideMenuOption),
			o => o.HasSideMenuOption,
			(o, v) => o.HasSideMenuOption = v);

	private ContentPresenter? _header;
	private Button? _actionButton;
	private ContentPresenter? _itemsContentPresenter;

	private object? _pendingHeader;

	private ICommand _backCommand;
	public ICommand BackCommand
	{
		get => _backCommand;
		set
		{
			if (SetAndRaise(BackCommandProperty, ref _backCommand, value))
				UpdateButtons();
		}
	}

	private ICommand _sideMenuCommand;
	public ICommand SideMenuCommand
	{
		get => _sideMenuCommand;
		set
		{
			if (SetAndRaise(SideMenuCommandProperty, ref _sideMenuCommand, value))
				UpdateButtons();
		}
	}

	private bool _hasSideMenuOption = true;
	public bool HasSideMenuOption
	{
		get => _hasSideMenuOption;
		set
		{
			if (SetAndRaise(HasSideMenuOptionProperty, ref _hasSideMenuOption, value))
				UpdateButtons();
		}
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
			? BackCommand
			: SideMenuCommand;

		if (hasItem)
		{
			_actionButton.Classes.Remove("FlyoutButton");
			_actionButton.Classes.Add("BackButton");
		}
		else
		{
			_actionButton.Classes.Remove("BackButton");
			_actionButton.Classes.Add("FlyoutButton");

			_actionButton.IsVisible = HasSideMenuOption;
		}
	}

	protected virtual void UpdateItems(object view, ContentPresenter itemPresenter)
	{
		itemPresenter.Content = view is INavigation navigation ? navigation.Item : null;
	}
}
