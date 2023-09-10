using Avalonia.Animation;
using Avalonia.Collections;
using AvaloniaInside.Shell.Platform;
using AvaloniaInside.Shell.Platform.Android;
using AvaloniaInside.Shell.Platform.Ios;
using AvaloniaInside.Shell.Platform.Windows;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellExample.ViewModels;
public class SettingViewModel : ViewModelBase
{
    public class TransitionItem
    {
        public string Name { get; set; }
        public IPageTransition Transition { get; set; }
    }

    public AvaloniaList<TransitionItem> Transitions { get; }

    private TransitionItem _currentTransition;
    public TransitionItem CurrentTransition
    {
        get => _currentTransition;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentTransition, value);
            if (MainViewModel != null)
                MainViewModel.CurrentTransition = value.Transition;
        }
    }

    public MainViewModel? MainViewModel { get; internal set; }

    public SettingViewModel()
    {
        Transitions = new AvaloniaList<TransitionItem>
        {
            new TransitionItem{ Name = "Android Default", Transition = AndroidDefaultPageSlide.Instance },
            new TransitionItem{ Name = "Android Material", Transition = MaterialListPageSlide.Instance },
            new TransitionItem{ Name = "iOS", Transition = DefaultIosPageSlide.Instance },
            new TransitionItem{ Name = "Windows EntranceNavigation", Transition = EntranceNavigationTransition.Instance },
            new TransitionItem{ Name = "Windows DrillInNavigation", Transition = DrillInNavigationTransition.Instance },
            new TransitionItem{ Name = "Windows ListSlideNavigation", Transition = ListSlideNavigationTransition.Instance },
        };

        CurrentTransition = Transitions.FirstOrDefault(f => f.Transition == PlatformSetup.TransitionForPage());
    }

}
