using Avalonia.Media;

namespace AvaloniaInside.Shell.Data;

public class SideMenuItem : IItem
{
	public required string Title { get; set; }

	public required string Path { get; set; }

	public IImage? Icon { get; set; }
}
