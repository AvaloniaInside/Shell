using ShellExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellExample.ViewModels.ShopViewModels;
public class ProductDetailViewModel : ViewModelBase
{
    public ProductDto Product { get; set; }

	public ProductDetailViewModel(ProductDto product)
	{
        Product = product;
    }
}
