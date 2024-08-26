namespace AvaloniaInside.Shell;

public interface INavigationBarProvider
{
	NavigationBar? NavigationBar { get; }
	NavigationBar? AttachedNavigationBar { get; }
}
