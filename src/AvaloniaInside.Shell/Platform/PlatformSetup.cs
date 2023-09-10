using Avalonia.Animation;
using AvaloniaInside.Shell.Platform.Android;
using AvaloniaInside.Shell.Platform.Ios;
using AvaloniaInside.Shell.Platform.Windows;
using System;

namespace AvaloniaInside.Shell.Platform;
public class PlatformSetup
{
    public static IPageTransition TransitionForPage()
    {
        if (OperatingSystem.IsAndroid())
            return new AndroidDefaultPageSlide();
        if (OperatingSystem.IsIOS())
            return new DefaultIosPageSlide();
        if (OperatingSystem.IsWindows())
            return new EntranceNavigationTransition();

        //Default for the moment
        return new DrillInNavigationTransition();
    }

    public static IPageTransition TransitionForList()
    {
        if (OperatingSystem.IsAndroid())
            return new MaterialListPageSlide();
        if (OperatingSystem.IsIOS())
            return new DefaultIosPageSlide();

        //Default for the moment
        return new ListSlideNavigationTransition();
    }

    public static IPageTransition? TransitionForTab()
    {
        if (OperatingSystem.IsIOS()) return null;
        if (OperatingSystem.IsMacOS()) return null;
        if (OperatingSystem.IsMacCatalyst()) return null;

        //Default for the moment
        return new MaterialListPageSlide();
    }
}
