using System.Threading;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using ShellExample.ViewModels.ShopViewModels;

namespace ShellExample.Views.ShopViews;

public partial class ProductCatalogFilterView : Page
{
	public ProductCatalogFilterViewModel ViewModel { get; internal set; }

	public ProductCatalogFilterView()
	{
		InitializeComponent();
	}

	public override Task InitialiseAsync(CancellationToken cancellationToken)
	{
		DataContext = ViewModel = new ProductCatalogFilterViewModel(Navigator);
		return base.InitialiseAsync(cancellationToken);
	}

    private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override Task ArgumentAsync(object? args, CancellationToken cancellationToken)
	{
		if (args is string selectedCategory)
			ViewModel.SetSelectedCategory(selectedCategory == string.Empty ? null : selectedCategory);
		return Task.CompletedTask;
	}
}
