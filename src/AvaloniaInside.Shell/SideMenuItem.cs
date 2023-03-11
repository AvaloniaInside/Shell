using Avalonia.Media;

namespace AvaloniaInside.Shell;

public class SideMenuItem
{
	public string Title { get; }
	public string Path { get; }
	public string? IconPath { get; }
	public IImage? IconSource { get; }

	public SideMenuItem(string title, string path, string? icon)
	{
		Title = title;
		Path = path;
		IconPath = icon;
	}

	public SideMenuItem(string title, string path, IImage? icon)
	{
		Title = title;
		Path = path;
		IconSource = icon;
	}
}
