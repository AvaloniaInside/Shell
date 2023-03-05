using System;
using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationChain
{
	public NavigationNode Node { get; set; } = default!;
	public object Instance { get; set; } = default!;
	public NavigateType Type { get; set; }
	public Uri Uri { get; set; } = default!;
	public NavigationChain? Back { get; set; }
	public bool Hosted { get; set; } = false;

	public IEnumerable<NavigationChain> GetAscendingNodes()
	{
		yield return this;
		if (Back == null) yield break;

		foreach (var node in Back.GetAscendingNodes())
			yield return node;
	}
}
