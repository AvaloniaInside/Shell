using System;
using System.Collections;
using Avalonia.Controls;

namespace AvaloniaInside.Shell;

public class HostedTabPage : Page, ISelectableHostedItems
{
	public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

	public IEnumerable? ItemsSource { get; set; }
	public ItemCollection Items { get; }
	public object? SelectedItem { get; set; }
}
