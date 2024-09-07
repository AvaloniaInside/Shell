using System;
using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationChain
{
	public NavigationNode Node { get; internal set; } = default!;
	public object Instance { get; internal set; } = default!;
	public NavigateType Type { get; internal set; }
	public Uri Uri { get; internal set; } = default!;
	public NavigationChain? Back { get; internal set; }
	public bool Hosted { get; internal set; }

	public IEnumerable<NavigationChain> GetAscendingNodes()
	{
		yield return this;
		if (Back == null) yield break;

		foreach (var node in Back.GetAscendingNodes())
			yield return node;
	}
}
