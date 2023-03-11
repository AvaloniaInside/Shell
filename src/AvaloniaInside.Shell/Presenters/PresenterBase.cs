using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaInside.Shell.Presenters;

public abstract class PresenterBase : IPresenter
{
	protected object GetHostControl(NavigationChain chain)
	{
		if (!chain.Hosted)
			return chain.Instance;

		var current = chain;
		while (current != null)
		{
			if (current.Back is HostNavigationChain { Instance: ItemsControl itemsControl } parent)
			{
				if (itemsControl.Items is not IList collection)
				{
					itemsControl.Items = collection = new AvaloniaList<object>();
				}

				foreach (var hostedChildChain in parent.Nodes.Where(hostedChildChain =>
					         !collection.Contains(hostedChildChain)))
				{
					collection.Add(hostedChildChain);
				}

				if (itemsControl is SelectingItemsControl selectingItemsControl)
				{
					selectingItemsControl.SelectedItem = current;
				}
			}
			else
			{
				break;
			}

			current = current.Back;
		}

		return current?.Instance ?? chain.Instance;
	}

	public abstract Task PresentAsync(ShellView shellView, NavigationChain chain, CancellationToken cancellationToken);
}
