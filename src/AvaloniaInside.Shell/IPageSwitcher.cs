using Avalonia;
using Avalonia.Animation;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;
public interface IPageSwitcher
{
    void SwitchPage(PageSwitcherInfo pageSwitcherArgs);
}

public record PageSwitcherInfo(
    object To,
    object? Sender,
    Point? Location,
    IPageTransition? OverrideTransition,
    bool WithAnimation,
    NavigateType Navigate);

