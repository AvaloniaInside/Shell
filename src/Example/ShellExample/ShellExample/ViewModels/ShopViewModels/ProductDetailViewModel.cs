using ShellExample.Models;

namespace ShellExample.ViewModels.ShopViewModels;
public class ProductDetailViewModel : ViewModelBase
{
    public ProductDto Product { get; set; }

	public ProductDetailViewModel(ProductDto product)
	{
        Product = product;
    }
}
