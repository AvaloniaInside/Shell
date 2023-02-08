using System;

namespace AvaloniaInside.Shell;

public class DefaultNavigationViewLocator : INavigationViewLocator
{
	public object GetView(NavigationNode navigationItem) =>
		Activator.CreateInstance(navigationItem.Page);
}