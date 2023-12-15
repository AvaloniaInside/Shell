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
        if (args is not ProductDto dto) return Task.CompletedTask;

        DataContext = new ProductDetailViewModel(dto);
        return Task.CompletedTask;
    }

    public override async Task OnNavigatingAsync(NaviagatingEventArgs args, CancellationToken cancellationToken)
    {
        if (args.Navigate == NavigateType.Pop)
        {
            var result = await Navigator.NavigateAndWaitAsync("/main/product/confirmation");
            if (result.Argument is bool v)
                args.Cancel = !v;
        }
    }
}
