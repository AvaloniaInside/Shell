using System;
using Avalonia.Collections;
using Avalonia.Metadata;

namespace AvaloniaInside.Shell.Data;

public class Route : IItem
{
	public string Path { get; set; }
	public Type Page { get; set; }
	public NavigateType Type { get; set; } = NavigateType.Normal;

	[Content] public AvaloniaList<Route> Routes { get; set; } = new();
}
