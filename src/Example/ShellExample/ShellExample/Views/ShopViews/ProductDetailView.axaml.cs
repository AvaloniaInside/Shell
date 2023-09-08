using Avalonia.Controls;
using AvaloniaInside.Shell;
using ShellExample.Models;
using ShellExample.ViewModels.ShopViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ShellExample.Views.ShopViews;
public partial class ProductDetailView : Page
{
    public ProductDetailView()
    {
        InitializeComponent();
    }

    public override Task ArgumentAsync(object args, CancellationToken cancellationToken)
    {
        DataContext = new ProductDetailViewModel((ProductDto)args);
        return Task.CompletedTask;
    }
}
