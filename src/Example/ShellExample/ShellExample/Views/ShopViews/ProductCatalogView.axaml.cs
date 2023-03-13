using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using ShellExample.ViewModels.ShopViewModels;

namespace ShellExample.Views.ShopViews;

public partial class ProductCatalogView : UserControl, INavigation
{
	public ProductCatalogView()
	{
		InitializeComponent();
		DataContext = ViewModel = new ProductCatalogViewModel(MainView.Current.ShellViewMain.Navigation);
	}

	public ProductCatalogViewModel ViewModel { get; }

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/tag-solid.png";
	public object? Title => string.IsNullOrEmpty(ViewModel.SelectedCategory)
		? "Products"
		: ViewModel.SelectedCategory;
	public object? Item => new Button()
	{
		Content = "Filter",
		Command = ViewModel.FilterCommand
	};
}
