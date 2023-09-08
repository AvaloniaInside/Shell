using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaInside.Shell;
public class Page : UserControl, INavigationLifecycle
{
    public ShellView? Shell { get; internal set; }
    public INavigator? Navigator => Shell?.Navigator;

    public virtual Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task ArgumentAsync(object args, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task TerminateAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
