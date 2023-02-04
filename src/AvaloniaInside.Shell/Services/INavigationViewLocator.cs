namespace AvaloniaInside.Shell.Services;

public interface INavigationViewLocator
{
	object GetView(NavigationNode navigationItem);
}
