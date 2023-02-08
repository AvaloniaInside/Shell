using System;
using System.Collections.Generic;

namespace AvaloniaInside.Shell;

public class NavigationStack
{
	private NavigationChain? Current { get; set; }

	public NavigationStackChanges Push(NavigationNode node, object instance, NavigateType type) =>
		type switch
		{
			NavigateType.ReplaceRoot => PushReplaceRoot(node, instance, type),
			NavigateType.Normal => PushNormal(node, instance, type),
			NavigateType.Replace => PushReplace(node, instance, type),
			NavigateType.Top => PushTop(node, instance, type),
			NavigateType.Clear => PushClear(node, instance, type),
			_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
		};

	public NavigationStackChanges Pop()
	{
		if (Current?.Back == null) return new NavigationStackChanges();
		var old = Current;

		old = old.Back;
		old.Back = null;

		return new NavigationStackChanges
		{
			Removed = new List<NavigationChain> { old }
		};
	}

	private NavigationStackChanges PushReplaceRoot(NavigationNode node, object instance, NavigateType type)
	{
		var popList = new List<NavigationChain>();
		var chain = Current;

		while (chain != null)
		{
			popList.Add(chain);
			chain = chain.Back;
		}

		foreach (var pop in popList)
			pop.Back = null;

		Current = new NavigationChain { Node = node, Instance = instance, Type = type };
		return new NavigationStackChanges()
		{
			New = new List<NavigationChain> { Current },
			Removed = popList
		};
	}

	private NavigationStackChanges PushNormal(NavigationNode node, object instance, NavigateType type)
	{
		Current = new NavigationChain { Node = node, Instance = instance, Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			New = new List<NavigationChain> { Current }
		};
	}


	private NavigationStackChanges PushReplace(NavigationNode node, object instance, NavigateType type)
	{
		var pop = Current;

		Current = new NavigationChain { Node = node, Instance = instance, Type = type, Back = pop?.Back };
		return new NavigationStackChanges()
		{
			New = new List<NavigationChain> { Current },
			Removed = pop != null ? new List<NavigationChain> { pop } : null
		};
	}

	private NavigationStackChanges PushTop(NavigationNode node, object instance, NavigateType type)
	{
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

				current.Instance = instance;
				current.Type = type;
				return new NavigationStackChanges();
			}
			previous = current;
			current = current.Back;
		}

		Current = new NavigationChain { Node = node, Instance = instance, Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			New = new List<NavigationChain> { Current }
		};
	}

	private NavigationStackChanges PushClear(NavigationNode node, object instance, NavigateType type)
	{
		var current = Current;
		NavigationChain? previous = null;
		while (current != null)
		{
			if (current.Node == node)
			{
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

		Current = new NavigationChain { Node = node, Instance = instance, Type = type, Back = Current };
		return new NavigationStackChanges()
		{
			New = new List<NavigationChain> { Current }
		};
	}
}
