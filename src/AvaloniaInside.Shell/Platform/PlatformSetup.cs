using Avalonia.Animation;
using AvaloniaInside.Shell.Platform.Android;
using AvaloniaInside.Shell.Platform.Ios;
using AvaloniaInside.Shell.Platform.Windows;
using System;

namespace AvaloniaInside.Shell.Platform;
public class PlatformSetup
{
    public static IPageTransition TransitionForPage
    {
        get
        {
	        return DefaultIosPageSlide.Instance;

            if (OperatingSystem.IsAndroid())
                return AndroidDefaultPageSlide.Instance;
            if (OperatingSystem.IsIOS())
                return DefaultIosPageSlide.Instance;
            if (OperatingSystem.IsWindows())
                return EntranceNavigationTransition.Instance;

            //Default for the moment
            return DrillInNavigationTransition.Instance;
        }
    }

    public static IPageTransition TransitionForList
    {
        get
        {
	        return DefaultIosPageSlide.Instance;

            if (OperatingSystem.IsAndroid())
                return MaterialListPageSlide.Instance;
            if (OperatingSystem.IsIOS())
                return DefaultIosPageSlide.Instance;

            //Default for the moment
            return ListSlideNavigationTransition.Instance;
        }
    }

    public static IPageTransition? TransitionForTab
    {
        get
        {
            if (OperatingSystem.IsIOS()) return null;
            if (OperatingSystem.IsMacOS()) return null;
            if (OperatingSystem.IsMacCatalyst()) return null;

            //Default for the moment
            return MaterialListPageSlide.Instance;
        }
    }

    public static IPageTransition TransitionForModal
    {
	    get
	    {
		    return AlertTransition.Instance;

		    if (OperatingSystem.IsAndroid())
			    return AndroidDefaultPageSlide.Instance;
		    if (OperatingSystem.IsIOS())
			    return AlertTransition.Instance;
		    if (OperatingSystem.IsWindows())
			    return DrillInNavigationTransition.Instance;;

		    //Default for the moment
		    return DrillInNavigationTransition.Instance;
	    }
    }
}
