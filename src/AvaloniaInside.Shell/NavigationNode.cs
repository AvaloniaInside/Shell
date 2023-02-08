using System;
using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationNode
{
	private List<NavigationNode> _nodes = new();

	public NavigationNode(string route, Type page, NavigationNodeType type, NavigateType navigate)
	{
		Route = route;
		Page = page;
		Type = type;
		Navigate = navigate;
	}

	public string Route { get; }
	public Type Page { get; }
	public NavigationNodeType Type { get; }
	public NavigateType Navigate { get; }
	public NavigationNode? Parent { get; private set; }
	public IReadOnlyList<NavigationNode> Nodes => _nodes.AsReadOnly();

	public void AddNode(NavigationNode node)
	{
		if (node.Parent != null)
			throw new InvalidOperationException("This node already attached to another parent");

		node.Parent = this;
	}

	public void RemoveFromParent()
	{
		if (Parent == null) throw new InvalidOperationException("There is no parent");
		Parent._nodes.Remove(this);
		Parent = null;
	}

	public void RemoveNode(NavigationNode node)
	{
		if (node.Parent != this)
			throw new InvalidOperationException("This is not my node");

		_nodes.Remove(node);
		node.Parent = null;
	}
}
