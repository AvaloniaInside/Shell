using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaInside.Shell;

public class NavigationStack
{
	public NavigationChain? Current { get; set; }

	public NavigationStackChanges Push(NavigationNode node, NavigateType type, Uri uri,
		Func<NavigationNode, object> instance)
	{
		var chain = type switch
		{
			NavigateType.ReplaceRoot => PushReplaceRoot(node, type, instance),
			NavigateType.Normal or NavigateType.Modal => PushNormal(node, type, instance),
			NavigateType.Replace => PushReplace(node, type, instance),
			NavigateType.Top => PushTop(node, type, instance),
			NavigateType.Clear => PushClear(node, type, instance),
			NavigateType.Pop => Pop(node, type, uri, instance),
			NavigateType.HostedItemChange => HostedItemChange(node, type, uri, instance)
		};

		if (chain.Front != null)
			chain.Front.Uri = uri;

		if (chain.Front != null)
			AscendingHostVerification(chain.Front, instance);

		return chain;
	}

	private NavigationStackChanges Pop(
		NavigationNode node,
		NavigateType type,
		Uri uri,
		Func<NavigationNode, object> getInstance)
	{
		if (Current == null) return PushTop(node, type, getInstance);

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

		return PushTop(node, type, getInstance);
	}

	private NavigationStackChanges PushReplaceRoot(NavigationNode node, NavigateType type,
		Func<NavigationNode, object> getInstance)
	{
		var popList = new List<NavigationChain>();
		var chain = Current;
		var previous = Current;

		while (chain != null)
		{
			popList.Add(chain);
			chain = chain.Back;
		}

		foreach (var pop in popList)
			pop.Back = null;

		Current = new NavigationChain { Node = node, Instance = getInstance(node), Type = type };
		return new NavigationStackChanges()
		{
			Front = Current,
			Previous = previous,
			Removed = popList
		};
	}

	private NavigationStackChanges PushNormal(NavigationNode node, NavigateType type,
		Func<NavigationNode, object> getInstance)
	{
		Current = new NavigationChain { Node = node, Instance = getInstance(node), Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			Front = Current,
			Previous = Current.Back
		};
	}


	private NavigationStackChanges PushReplace(NavigationNode node, NavigateType type,
		Func<NavigationNode, object> getInstance)
	{
		var pop = Current;

		Current = new NavigationChain { Node = node, Instance = getInstance(node), Type = type, Back = pop?.Back };
		return new NavigationStackChanges()
		{
			Previous = pop,
			Front = Current,
			Removed = pop != null ? new List<NavigationChain> { pop } : null
		};
	}

	private NavigationStackChanges PushTop(NavigationNode node, NavigateType type,
		Func<NavigationNode, object> getInstance)
	{
		var previousChain = Current;
		var current = Current;
		NavigationChain? previous = null;
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

		Current = new NavigationChain { Node = node, Instance = getInstance(node), Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			Previous = previousChain,
			Front = Current
		};
	}

	private NavigationStackChanges PushClear(NavigationNode node, NavigateType type,
		Func<NavigationNode, object> getInstance)
	{
		var removedNodes = new List<NavigationChain>();
		var previousChain = Current;
		var current = Current;
		NavigationChain? previous = null;
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

		Current = new NavigationChain { Node = node, Instance = getInstance(node), Type = type, Back = Current };
		return new NavigationStackChanges
		{
			Previous = previousChain,
			Front = Current,
			Removed = removedNodes
		};
	}

	private NavigationStackChanges HostedItemChange(
		NavigationNode node,
		NavigateType type,
		Uri uri,
		Func<NavigationNode, object> getInstance)
	{
		var found = Current?.GetAscendingNodes()
			.OfType<HostNavigationChain>()
			.SelectMany(h => h.AggregatedNodes)
			.FirstOrDefault(f => f.Node == node);

		if (found != null)
		{
			Current = found;
			return new NavigationStackChanges() { Front = found };
		}

		//return PushNormal(node, type, getInstance);

		Current = new NavigationChain
		{
			Node = node,
			Instance = getInstance(node),
			Type = type,
			Back = Current
		};

		var firstReachHost = default(HostNavigationChain);
		foreach (var chain in Current.GetAscendingNodes())
		{
			if (chain is HostNavigationChain hostChain)
				firstReachHost = hostChain;
			else if (firstReachHost != null)
				break;
		}

		if (firstReachHost == null)
			return new NavigationStackChanges { Front = Current };

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

		return new NavigationStackChanges { Front = Current };
	}

	private void AscendingHostVerification(NavigationChain chain, Func<NavigationNode, object> getInstance)
	{
		var parentNode = chain.Node.Parent;
		if (parentNode == null) return;
		if (parentNode.Type == NavigationNodeType.Page) return;
		if (chain.Back?.Node == parentNode && chain.Back is HostNavigationChain verifyHostChain)
		{
			VerifyHostInitialised(chain, parentNode, verifyHostChain, getInstance);

			return;
		}

		var parentChain = new HostNavigationChain
		{
			Back = chain.Back,
			Node = parentNode,
			Type = NavigateType.Normal,
			Instance = getInstance(parentNode),
			Uri = new Uri(chain.Uri, parentNode.Route)
		};

		VerifyHostInitialised(chain, parentNode, parentChain, getInstance);

		chain.Back = parentChain;

		AscendingHostVerification(parentChain, getInstance);
	}

	private static void VerifyHostInitialised(
		NavigationChain chain,
		NavigationNode parentNode,
		HostNavigationChain parentChain,
		Func<NavigationNode, object> getInstance)
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
			hostChildChain.Instance = getInstance(hostChildNode);
			hostChildChain.Uri = new Uri(chain.Uri, hostChildNode.Route);
			hostChildChain.Hosted = true;

			parentChain.Nodes.Add(hostChildChain);
		}
	}
}
