using Avalonia.Animation;
using AvaloniaInside.Shell.Platform;
using ReactiveUI;

namespace ShellBottomCustomNavigator.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    private IPageTransition _currentTransition = PlatformSetup.TransitionForPage;
    public IPageTransition CurrentTransition
    {
        get => _currentTransition;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentTransition, value);
        }
    }
}
