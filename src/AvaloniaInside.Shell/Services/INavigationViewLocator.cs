using System;

namespace AvaloniaInside.Shell.Services;

public interface INavigationViewLocator
{
	object GetView(NavigationNode navigationItem);
}

public class DefaultNavigationViewLocator : INavigationViewLocator
{
	public object GetView(NavigationNode navigationItem) =>
		Activator.CreateInstance(navigationItem.Page);
}
