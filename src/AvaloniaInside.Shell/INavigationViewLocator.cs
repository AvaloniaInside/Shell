using System;

namespace AvaloniaInside.Shell;

public interface INavigationViewLocator
{
	object GetView(NavigationNode navigationItem);
}

public class DefaultNavigationViewLocator : INavigationViewLocator
{
	public object GetView(NavigationNode navigationItem) =>
		Activator.CreateInstance(navigationItem.Page);
}
