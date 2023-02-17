using System;

namespace AvaloniaInside.Shell;

public class DefaultNavigationViewLocator : INavigationViewLocator
{
	public object GetView(NavigationNode navigationItem) =>
		Activator.CreateInstance(navigationItem.Page)
		?? throw new TypeLoadException("Cannot create instance of page type");
}
