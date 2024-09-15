using Avalonia.Animation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;
public interface INavigatorLifecycle
{
    Task OnNavigatingAsync(NavigatingEventArgs args, CancellationToken cancellationToken);
    Task OnNavigateAsync(NavigateEventArgs args, CancellationToken cancellationToken);
}

public class NavigatingEventArgs : EventArgs
{
    public object? Sender { get; internal init; }
    public object? From { get; internal init; }
    public Uri? FromUri { get; internal init; }
    public Uri? ToUri { get; internal init; }
    public object? Argument { get; set; }
    public IPageTransition? OverrideTransition { get; set; }
    public bool WithAnimation { get; set; }
    public NavigateType Navigate { get; set; }
    public bool Cancel { get; set; } = false;
}

public class NavigateEventArgs : EventArgs
{
    public object? Sender { get; internal init; }
    public object? From { get; internal init; }
    public object? To { get; internal set; }
    public Uri? FromUri { get; internal init; }
    public Uri? ToUri { get; internal init; }
    public object? Argument { get; internal init; }
    public bool WithAnimation { get; internal init; }
    public NavigateType Navigate { get; internal init; }
}
