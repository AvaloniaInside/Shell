using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell;

public class SideMenuView : TemplatedControl
{
	private ContentPresenter? _header;
	private ListBox? _listBox;
	private ContentPresenter? _footer;

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		base.OnApplyTemplate(e);
		_header = e.NameScope.Find<ContentPresenter>("PART_Header");
		_listBox = e.NameScope.Find<ListBox>("PART_Items")
		           ?? throw new KeyNotFoundException("PART_Items not found in SideMenuView template");
		_footer = e.NameScope.Find<ContentPresenter>("PART_Footer");

		SetupUi();
	}

	private void SetupUi()
	{
		_listBox!.Items ??= new ObservableCollection<object>();
		_listBox!.SelectionChanged += OnSelectionChanged;
	}

	private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
	{

	}
}
