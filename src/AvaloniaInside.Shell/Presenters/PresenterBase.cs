using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;

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
			if (current.Back is HostNavigationChain parent &&
			    HostedItemsHelper.GetHostedItems(current.Back?.Instance) is { } hostedItems)
			{
				if ((hostedItems.Items ?? hostedItems.ItemsSource) is not IList collection)
				{
					hostedItems.ItemsSource = collection = new AvaloniaList<object>();
				}

				foreach (var hostedChildChain in parent.Nodes.Where(hostedChildChain =>
					         !collection.Contains(hostedChildChain)))
				{
					collection.Add(hostedChildChain);
				}

				if (hostedItems is ISelectableHostItems selectingItemsControl)
					selectingItemsControl.SelectedItem = current;
			}
			else
			{
				break;
			}

			current = current.Back;
		}

		return current?.Instance ?? chain.Instance;
	}

	public abstract Task PresentAsync(
		ShellView shellView,
		NavigationChain chain,
		NavigateType navigateType,
		NavigateEventArgs eventArgs,
        CancellationToken cancellationToken);
}
