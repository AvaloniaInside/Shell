using System.Collections;
using Avalonia.Controls;

namespace AvaloniaInside.Shell;

public interface IHostedItems
{
	IEnumerable? ItemsSource { get; set; }
	ItemCollection Items { get; }
}