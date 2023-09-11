using Avalonia.Controls;
using Avalonia.VisualTree;
using AvaloniaInside.Shell.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellExample;
public static class Helper
{
    public static void SetCarouselToTabControl(Carousel carousel, bool v)
    {
        var tabControl = carousel.FindAncestorOfType<TabControl>();
        if (tabControl == null)
        {
            carousel.AttachedToVisualTree += delegate { SetCarouselToTabControl(carousel, true); };
            return;
        }

        carousel.ItemsSource = tabControl.Items;
    }
}
