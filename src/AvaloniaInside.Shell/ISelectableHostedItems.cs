using System;
using Avalonia.Controls;

namespace AvaloniaInside.Shell;

public interface ISelectableHostedItems : IHostedItems
{
	event EventHandler<SelectionChangedEventArgs> SelectionChanged;
	object? SelectedItem { get; set; }
}
