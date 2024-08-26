using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaInside.Shell;
using DynamicData;
using ReactiveUI;
using ShellBottomNavigator.Models;

namespace ShellBottomNavigator.ViewModels.ShopViewModels;

public class ProductCatalogViewModel : ViewModelBase
{
	private readonly INavigator _navigationService;
	private string? _selectedCategory;

	public ObservableCollection<ProductDto> Products { get; }
	public ICommand FilterCommand { get; }

	public string? SelectedCategory
	{
		get => _selectedCategory;
		set
		{
			this.RaiseAndSetIfChanged(ref _selectedCategory, value);
			this.RaisePropertyChanged(nameof(Title));
		}
	}

	public string? Title => string.IsNullOrEmpty(SelectedCategory) ? "Products" : SelectedCategory;

	public ProductCatalogViewModel(INavigator navigationService)
	{
		_navigationService = navigationService;
		Products = new ObservableCollection<ProductDto>(DummyPlace.Products);
		FilterCommand = ReactiveCommand.CreateFromTask(FilterAsync);
	}

	private async Task FilterAsync(CancellationToken cancellationToken)
	{
		var result = await _navigationService
			.NavigateAndWaitAsync("/main/product/filter", _selectedCategory, cancellationToken);

		if (result.HasArgument)
			UpdateSelectedCategory(result.As<string>());
	}

	private void UpdateSelectedCategory(string? selectedCategory)
	{
		if (selectedCategory == string.Empty) selectedCategory = null;

		var filtered = selectedCategory == null
			? DummyPlace.Products
			: DummyPlace.Products.Where(w => w.MainCategory == selectedCategory);
		Products.Clear();
		Products.AddRange(filtered);
		SelectedCategory = selectedCategory;
	}
}
