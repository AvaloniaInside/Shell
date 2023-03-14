using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShellExample.ViewModels.ShopViewModels;

namespace ShellExample.Views.ShopViews;

public partial class ProductCatalogView : UserControl
{
	public ProductCatalogView()
	{
		InitializeComponent();
		DataContext = ViewModel = new ProductCatalogViewModel(MainView.Current.ShellViewMain.Navigator);
	}

	public ProductCatalogViewModel ViewModel { get; }

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/tag-solid.png";
}

