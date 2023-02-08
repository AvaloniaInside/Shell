namespace AvaloniaInside.Shell;

public interface INavigationViewLocator
{
	object GetView(NavigationNode navigationItem);
}