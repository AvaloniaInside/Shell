using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class NavigationBar : TemplatedControl
{
	public static readonly DirectProperty<NavigationBar, ICommand> SideMenuCommandProperty =
		AvaloniaProperty.RegisterDirect<NavigationBar, ICommand>(
			nameof(SideMenuCommand),
			o => o.SideMenuCommand,
			(o, v) => o.SideMenuCommand = v);

	public static readonly DirectProperty<NavigationBar, ICommand> BackCommandProperty =
		AvaloniaProperty.RegisterDirect<NavigationBar, ICommand>(
			nameof(BackCommand),
			o => o.BackCommand,
			(o, v) => o.BackCommand = v);

	public static readonly DirectProperty<NavigationBar, bool> HasSideMenuOptionProperty =
		AvaloniaProperty.RegisterDirect<NavigationBar, bool>(
			nameof(HasSideMenuOption),
			o => o.HasSideMenuOption,
			(o, v) => o.HasSideMenuOption = v);

	private ContentControl? _header;
	private Button? _actionButton;
	private ContentControl? _items;

	private object? _pendingHeader;

	public ShellView? ShellView { get; internal set; }

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

	public static readonly AttachedProperty<object> ItemProperty =
		AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, object>("Item");

	public static object GetItem(AvaloniaObject element) =>
		element.GetValue(ItemProperty);

	public static void SetItem(AvaloniaObject element, object parameter) =>
		element.SetValue(ItemProperty, parameter);

	public static readonly AttachedProperty<object> HeaderProperty =
		AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, object>("Header");

	public static object GetHeader(AvaloniaObject element) =>
		element.GetValue(HeaderProperty);

	public static void SetHeader(AvaloniaObject element, object parameter) =>
		element.SetValue(HeaderProperty, parameter);

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		_header = e.NameScope.Find<ContentControl>("PART_Header") ?? throw new ArgumentNullException("PART_Header");
		_actionButton = e.NameScope.Find<Button>("PART_ActionButton");
		_items = e.NameScope.Find<ContentControl>("PART_Items");

		if (_actionButton != null)
			_actionButton.Command = _backCommand;

		if (_pendingHeader != null)
			_ = UpdateAsync(_pendingHeader, CancellationToken.None);
	}

	public Task UpdateAsync(object view, CancellationToken cancellationToken)
	{
		if (_header == null && _items == null)
		{
			_pendingHeader = view;
			return Task.CompletedTask;
		}

		if (_header != null)
			UpdateHeader(view, _header);

		if (_items != null)
			UpdateItems(view, _items);

		UpdateButtons();
		return Task.CompletedTask;
	}

	protected virtual void UpdateButtons()
	{
		if (ShellView == null) return;

		var navService = ShellView.Navigator;
		var hasItem = navService?.HasItemInStack() ?? false;

		if (_actionButton == null) return;

		_actionButton.Command = hasItem
			? BackCommand
			: SideMenuCommand;

		if (hasItem)
		{
			_actionButton.Classes.Remove("FlyoutButton");
			_actionButton.Classes.Add("BackButton");

			_actionButton.IsVisible = true;
		}
		else
		{
			_actionButton.Classes.Remove("BackButton");
			_actionButton.Classes.Add("FlyoutButton");

			_actionButton.IsVisible = HasSideMenuOption;
		}
	}

	protected virtual void UpdateItems(object view, ContentControl itemPresenter)
	{
		if (view is not AvaloniaObject)
		{
			itemPresenter.Content = null;
			itemPresenter.DataContext = null;
			return;
		}

		if (view is StyledElement element)
		{
			itemPresenter[!ContentControl.ContentProperty] = element[!ItemProperty];
			itemPresenter.DataContext = element.DataContext ?? element;
			return;
		}

		itemPresenter.Content = view;
	}

	protected virtual void UpdateHeader(object view, ContentControl itemPresenter)
	{
		if (view is not AvaloniaObject)
		{
			itemPresenter.Content = null;
			itemPresenter.DataContext = null;
			return;
		}

		if (view is StyledElement element)
		{
			itemPresenter.DataContext = element.DataContext ?? element;
			itemPresenter[!ContentControl.ContentProperty] = element[!HeaderProperty];
			return;
		}

		itemPresenter.Content = view;
	}
}
