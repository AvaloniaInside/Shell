using Avalonia.Animation;
using AvaloniaInside.Shell.Platform;

namespace ShellExample.ViewModels;

public class MainViewModel : ViewModelBase
{
	public string Greeting => "Welcome to Avalonia!";

    private IPageTransition _currentTransition = PlatformSetup.TransitionForPage;
    public IPageTransition CurrentTransition
    {
        get => _currentTransition;
        set
        {
            SetField(ref _currentTransition, value);
        }
    }
}
