using System;
using Avalonia.Controls;

namespace AvaloniaInside.Shell;

public interface ISelectableHostItems : IHostItems
{
	event EventHandler<SelectionChangedEventArgs> SelectionChanged;
	object? SelectedItem { get; set; }
}
