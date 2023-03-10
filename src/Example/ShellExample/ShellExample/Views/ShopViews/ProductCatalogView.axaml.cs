using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using ShellExample.ViewModels.ShopViewModels;

namespace ShellExample.Views.ShopViews;

public partial class ProductCatalogView : UserControl, INavigation, INavigationLifecycle
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

	public string Icon => "fa-solid fa-tag";
	public object? Title => string.IsNullOrEmpty(ViewModel.SelectedCategory)
		? "Products"
		: ViewModel.SelectedCategory;
	public object? Item => new Button()
	{
		Content = "Filter",
		Command = ViewModel.FilterCommand
	};

	public Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public Task StartAsync(CancellationToken cancellationToken)=> Task.CompletedTask;

	public Task StopAsync(CancellationToken cancellationToken)=> Task.CompletedTask;

	public Task ArgumentAsync(object args, CancellationToken cancellationToken)
	{
		if (args is string selectedCategory)
			ViewModel.UpdateSelectedCategory(selectedCategory == string.Empty ? null : selectedCategory);

		return Task.CompletedTask;
	}

	public Task TerminateAsync(CancellationToken cancellationToken)=> Task.CompletedTask;
}

