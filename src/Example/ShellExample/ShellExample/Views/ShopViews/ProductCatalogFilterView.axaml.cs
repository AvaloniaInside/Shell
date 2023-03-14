using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using ShellExample.ViewModels.ShopViewModels;

namespace ShellExample.Views.ShopViews;

public partial class ProductCatalogFilterView : UserControl, INavigationLifecycle
{
	public ProductCatalogFilterViewModel ViewModel { get; }

	public ProductCatalogFilterView()
	{
		InitializeComponent();
		DataContext = ViewModel = new ProductCatalogFilterViewModel(MainView.Current.ShellViewMain.Navigator);
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public Task InitialiseAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task AppearAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task DisappearAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task ArgumentAsync(object args, CancellationToken cancellationToken)
	{
		if (args is string selectedCategory)
			ViewModel.SetSelectedCategory(selectedCategory == string.Empty ? null : selectedCategory);
		return Task.CompletedTask;
	}

	public Task TerminateAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
