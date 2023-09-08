using Avalonia.Animation;
using Avalonia.Collections;
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
            new TransitionItem{ Name = "Android Default", Transition = new AndroidDefaultPageSlide() },
            new TransitionItem{ Name = "Android Material", Transition = new MaterialListPageSlide() },
            new TransitionItem{ Name = "iOS", Transition = new DefaultIosPageSlide() },
            new TransitionItem{ Name = "Windows EntranceNavigation", Transition = new EntranceNavigationTransition() },
            new TransitionItem{ Name = "Windows DrillInNavigation", Transition = new DrillInNavigationTransition() },
            new TransitionItem{ Name = "Windows ListSlideNavigation", Transition = new ListSlideNavigationTransition() },
        };

        CurrentTransition = Transitions[4];
    }

}
