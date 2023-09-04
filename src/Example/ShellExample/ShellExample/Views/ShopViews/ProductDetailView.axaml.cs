using Avalonia.Controls;
using AvaloniaInside.Shell;
using ShellExample.Models;
using ShellExample.ViewModels.ShopViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ShellExample.Views.ShopViews;
public partial class ProductDetailView : UserControl, INavigationLifecycle
{
    public ProductDetailView()
    {
        InitializeComponent();
    }

    public Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        DataContext = new ProductDetailViewModel((ProductDto)args);
        return Task.CompletedTask;
    }

    public Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task TerminateAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
