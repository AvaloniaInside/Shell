using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaInside.Shell;

public class NavigationStack(INavigationViewLocator viewLocator)
{
	public NavigationChain? Current { get; set; }

	public NavigationStackChanges Push(
		NavigationNode node,
		NavigateType type,
		Uri uri)
	{
		var changes = type switch
		{
			NavigateType.ReplaceRoot => PushReplaceRoot(node, type),
			NavigateType.Normal or NavigateType.Modal => PushNormal(node, type),
			NavigateType.Replace => PushReplace(node, type),
			NavigateType.Top => PushTop(node, type),
			NavigateType.Clear => PushClear(node, type),
			NavigateType.Pop => Pop(node, type, uri),
			NavigateType.HostedItemChange => HostedItemChange(node, type, uri)
		};

		if (changes.Front != null)
			changes.Front.Uri = uri;

		if (changes.Front != null)
			AscendingHostVerification(changes, changes.Front);

		return changes;
	}

	private NavigationStackChanges Pop(
		NavigationNode node,
		NavigateType type,
		Uri uri)
	{
		if (Current == null) return PushTop(node, type);

		var previous = Current;
		var list = new List<NavigationChain>();

		foreach (var chain in Current.GetAscendingNodes())
		{
			if (chain.Node == node)
			{
				Current = chain;
				return new NavigationStackChanges
				{
					Front = chain,
					Removed = list,
					Previous = previous
				};
			}

			list.Add(chain);
		}

		return PushTop(node, type);
	}

	private NavigationStackChanges PushReplaceRoot(NavigationNode node, NavigateType type)
	{
		var popList = new List<NavigationChain>();
		var chain = Current;
		var previous = Current;
		var changes = new NavigationStackChanges();

		while (chain != null)
		{
			popList.Add(chain);
			chain = chain.Back;
		}

		foreach (var pop in popList)
			pop.Back = null;

		Current = NewInstanceAndChain(changes, node, type, null);

		changes.Front = Current;
		changes.Previous = previous;
		changes.Removed = popList;

		return changes;
	}

	private NavigationStackChanges PushNormal(
		NavigationNode node,
		NavigateType type)
	{
		var changes = new NavigationStackChanges();

		Current = NewInstanceAndChain(changes, node, type, Current);
		changes.Front = Current;
		changes.Previous = Current.Back;

		return changes;
	}


	private NavigationStackChanges PushReplace(NavigationNode node, NavigateType type)
	{
		var pop = Current;
		var changes = new NavigationStackChanges();

		Current = NewInstanceAndChain(changes, node, type, pop?.Back);

		changes.Front = Current;
		changes.Previous = pop;
		changes.Removed = pop != null ? new List<NavigationChain> { pop } : null;

		return changes;
	}

	private NavigationStackChanges PushTop(NavigationNode node, NavigateType type)
	{
		var previousChain = Current;
		var current = Current;
		NavigationChain? previous = null;
		var changes = new NavigationStackChanges();

		while (current != null)
		{
			if (current.Node == node)
			{
				if (previous != null)
				{
					previous.Back = current.Back;
					current.Back = Current;
					Current = current;
				}

				current.Type = type;
				return new NavigationStackChanges
				{
					Previous = previousChain,
					Front = Current
				};
			}

			previous = current;
			current = current.Back;
		}

		Current = NewInstanceAndChain(changes, node, type, Current);

		changes.Front = Current;
		changes.Previous = previousChain;

		return changes;
	}

	private NavigationStackChanges PushClear(NavigationNode node, NavigateType type)
	{
		var removedNodes = new List<NavigationChain>();
		var previousChain = Current;
		var current = Current;
		NavigationChain? previous = null;
		var changes = new NavigationStackChanges();

		while (current != null)
		{
			if (current.Node == node)
			{
				removedNodes.Add(current);
				if (previous != null)
				{
					previous.Back = current.Back;
				}
				else if (Current != null)
				{
					Current.Back = current.Back;
				}
			}
			else
			{
				previous = current;
			}

			current = current.Back;
		}

		Current = NewInstanceAndChain(changes, node, type, Current);

		changes.Front = Current;
		changes.Previous = previousChain;
		changes.Removed = removedNodes;

		return changes;
	}

	private NavigationStackChanges HostedItemChange(
		NavigationNode node,
		NavigateType type,
		Uri uri)
	{
		var changes = new NavigationStackChanges();
		var found = Current?.GetAscendingNodes()
			.OfType<HostNavigationChain>()
			.SelectMany(h => h.AggregatedNodes)
			.FirstOrDefault(f => f.Node == node);

		if (found != null)
		{
			Current = found;
			changes.Front = found;
			return changes;
		}

		Current = NewInstanceAndChain(changes, node, type, Current);

		var firstReachHost = default(HostNavigationChain);
		foreach (var chain in Current.GetAscendingNodes())
		{
			if (chain is HostNavigationChain hostChain)
				firstReachHost = hostChain;
			else if (firstReachHost != null)
				break;
		}

		if (firstReachHost == null)
		{
			changes.Front = Current;
			return changes;
		}

		var all = firstReachHost.AggregatedNodes.ToList();
		var lastChainUpdated = Current;
		foreach (var parentNode in node.GetAscendingNodes().Skip(1))
		{
			var foundChain = all.FirstOrDefault(f => f.Node == parentNode);
			if (foundChain != null)
			{
				lastChainUpdated.Back = foundChain;
				lastChainUpdated = foundChain;
			}
		}

		changes.Front = Current;
		return changes;
	}

	private void AscendingHostVerification(NavigationStackChanges changes, NavigationChain chain)
	{
		var parentNode = chain.Node.Parent;
		if (parentNode == null) return;
		if (parentNode.Type == NavigationNodeType.Page) return;
		if (chain.Back?.Node == parentNode && chain.Back is HostNavigationChain verifyHostChain)
		{
			VerifyHostInitialised(changes, chain, parentNode, verifyHostChain);

			return;
		}

		var parentChain = new HostNavigationChain
		{
			Back = chain.Back,
			Node = parentNode,
			Type = NavigateType.Normal,
			Instance = viewLocator.GetView(parentNode),
			Uri = new Uri(chain.Uri, parentNode.Route)
		};
		changes.NewNavigationChains.Add(parentChain);

		VerifyHostInitialised(changes, chain, parentNode, parentChain);

		chain.Back = parentChain;

		AscendingHostVerification(changes, parentChain);
	}

	private void VerifyHostInitialised(
		NavigationStackChanges changes,
		NavigationChain chain,
		NavigationNode parentNode,
		HostNavigationChain parentChain)
	{
		foreach (var hostChildNode in parentNode.Nodes)
		{
			if (parentChain.Nodes.Any(a => a.Node == hostChildNode))
				continue;

			if (hostChildNode == chain.Node)
			{
				chain.Hosted = true;
				parentChain.Nodes.Add(chain);
				continue;
			}

			var hostChildChain = hostChildNode.Type == NavigationNodeType.Host
				? new HostNavigationChain()
				: new NavigationChain();

			hostChildChain.Back = parentChain;
			hostChildChain.Node = hostChildNode;
			hostChildChain.Type = NavigateType.Normal;
			hostChildChain.Instance = viewLocator.GetView(hostChildNode);
			hostChildChain.Uri = new Uri(chain.Uri, hostChildNode.Route);
			hostChildChain.Hosted = true;

			changes.NewNavigationChains.Add(hostChildChain);

			parentChain.Nodes.Add(hostChildChain);
		}
	}

	private NavigationChain NewInstanceAndChain(
		NavigationStackChanges changes,
		NavigationNode node,
		NavigateType type,
		NavigationChain? back)
	{
		var instance = viewLocator.GetView(node);
		var chain = new NavigationChain
		{
			Node = node,
			Instance = instance,
			Type = type,
			Back = back
		};
		changes.NewNavigationChains.Add(chain);
		return chain;
	}
}
