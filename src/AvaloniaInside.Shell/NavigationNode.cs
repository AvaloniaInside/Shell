using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaInside.Shell;

public class NavigationNode
{
	private List<NavigationNode> _nodes = new();

	public NavigationNode(string route, Type page, NavigationNodeType type, NavigateType navigate, string? defaultPath)
	{
		Route = route;
		Page = page;
		Type = type;
		Navigate = navigate;
		DefaultPath = defaultPath;
	}

	public string Route { get; }
	public Type Page { get; }
	public NavigationNodeType Type { get; }
	public NavigateType Navigate { get; }
	public NavigationNode? Parent { get; private set; }
	public string? DefaultPath { get; }
	public IReadOnlyList<NavigationNode> Nodes => _nodes.AsReadOnly();

	public NavigationNode? GetDefaultNode() =>
		Type != NavigationNodeType.Page
			? _nodes.FirstOrDefault(f => f.Route == DefaultPath) ?? _nodes.FirstOrDefault()
			: null;

	public NavigationNode? GetLastDefaultNode()
	{
		if (Type == NavigationNodeType.Page) return null;
		var defaultSubNode = _nodes.FirstOrDefault(f => f.Route == DefaultPath) ?? _nodes.FirstOrDefault();
		return defaultSubNode?.GetLastDefaultNode() ?? defaultSubNode;
	}

	public IEnumerable<NavigationNode> GetAscendingNodes()
	{
		yield return this;
		if (Parent == null) yield break;

		foreach (var node in Parent.GetAscendingNodes())
			yield return node;
	}

	internal void AddNode(NavigationNode node)
	{
		if (node.Parent != null)
			throw new InvalidOperationException("This node already attached to another parent");

		_nodes.Add(node);
		node.Parent = this;
	}

	internal void RemoveFromParent()
	{
		if (Parent == null)
			throw new InvalidOperationException("There is no parent");

		Parent._nodes.Remove(this);
		Parent = null;
	}

	internal void RemoveNode(NavigationNode node)
	{
		if (node.Parent != this)
			throw new InvalidOperationException("This is not my node");

		_nodes.Remove(node);
		node.Parent = null;
	}
}
