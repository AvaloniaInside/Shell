using System;
using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationStack
{
	public NavigationChain? Current { get; set; }

	public NavigationStackChanges Push(NavigationNode node, NavigateType type, Func<object> instance) =>
		type switch
		{
			NavigateType.ReplaceRoot => PushReplaceRoot(node, type, instance),
			NavigateType.Normal or NavigateType.Modal => PushNormal(node, type, instance),
			NavigateType.Replace => PushReplace(node, type, instance),
			NavigateType.Top => PushTop(node, type, instance),
			NavigateType.Clear => PushClear(node, type, instance),
			NavigateType.Pop => Pop(),
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};

	public NavigationStackChanges Pop(int depth = 1)
	{
		if (depth < 1) throw new ArgumentOutOfRangeException(nameof(depth), "depth cannot be less than 1");
		if (Current?.Back == null) return new NavigationStackChanges();

		if (depth == 1)
		{
			var old = Current;

			old = old.Back;
			old.Back = null;

			return new NavigationStackChanges
			{
				Removed = new List<NavigationChain> { old }
			};
		}
		else
		{
			var old = Current;
			var list = new List<NavigationChain>();

			for (var i = 0; i < depth; i++)
			{
				list.Add(old);
				if (old.Back == null)
					break;
			}

			foreach (var item in list)
				item.Back = null;

			return new NavigationStackChanges
			{
				Removed = list
			};
		}
	}

	private NavigationStackChanges PushReplaceRoot(NavigationNode node, NavigateType type, Func<object> getInstance)
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

		Current = new NavigationChain { Node = node, Instance = getInstance(), Type = type };
		return new NavigationStackChanges()
		{
			Front =  Current,
			Previous = previous,
			Removed = popList
		};
	}

	private NavigationStackChanges PushNormal(NavigationNode node, NavigateType type, Func<object> getInstance)
	{
		Current = new NavigationChain { Node = node, Instance = getInstance(), Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			Front =  Current,
			Previous = Current.Back
		};
	}


	private NavigationStackChanges PushReplace(NavigationNode node, NavigateType type, Func<object> getInstance)
	{
		var pop = Current;

		Current = new NavigationChain { Node = node, Instance = getInstance(), Type = type, Back = pop?.Back };
		return new NavigationStackChanges()
		{
			Previous = pop,
			Front = Current,
			Removed = pop != null ? new List<NavigationChain> { pop } : null
		};
	}

	private NavigationStackChanges PushTop(NavigationNode node, NavigateType type, Func<object> getInstance)
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

				current.Instance = getInstance();
				current.Type = type;
				return new NavigationStackChanges
				{
					Previous = previousChain
				};
			}

			previous = current;
			current = current.Back;
		}

		Current = new NavigationChain { Node = node, Instance = getInstance(), Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			Previous = previousChain,
			Front = Current
		};
	}

	private NavigationStackChanges PushClear(NavigationNode node, NavigateType type, Func<object> getInstance)
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
				else
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

		Current = new NavigationChain { Node = node, Instance = getInstance(), Type = type, Back = Current };
		return new NavigationStackChanges
		{
			Previous = previousChain,
			Front = Current,
			Removed = removedNodes
		};
	}
}
