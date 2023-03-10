using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using AvaloniaInside.Shell;
using DynamicData;
using ReactiveUI;
using ShellExample.Models;

namespace ShellExample.ViewModels.ShopViewModels;

public class ProductCatalogViewModel : ViewModelBase
{
	private readonly INavigationService _navigationService;
	private string? _selectedCategory;

	public ObservableCollection<ProductDto> Products { get; }
	public ICommand FilterCommand { get; }

	public string? SelectedCategory
	{
		get => _selectedCategory;
		set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
	}

	public ProductCatalogViewModel(INavigationService navigationService)
	{
		_navigationService = navigationService;
		Products = new ObservableCollection<ProductDto>(DummyPlace.Products);
		FilterCommand = ReactiveCommand.CreateFromTask(FilterAsync);
	}

	private Task FilterAsync(CancellationToken cancellationToken)
	{
		return _navigationService.NavigateAsync("/main/product/filter", _selectedCategory, cancellationToken);
	}

	public void UpdateSelectedCategory(string? selectedCategory)
	{
		var filtered = selectedCategory == null
			? DummyPlace.Products
			: DummyPlace.Products.Where(w => w.MainCategory == selectedCategory);
		Products.Clear();
		Products.AddRange(filtered);
		SelectedCategory = selectedCategory;
	}
}
