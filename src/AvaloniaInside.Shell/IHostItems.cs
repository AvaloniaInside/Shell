using System.Collections;
using Avalonia.Controls;

namespace AvaloniaInside.Shell;

public interface IHostItems
{
	IEnumerable? ItemsSource { get; set; }
	ItemCollection Items { get; }
}
