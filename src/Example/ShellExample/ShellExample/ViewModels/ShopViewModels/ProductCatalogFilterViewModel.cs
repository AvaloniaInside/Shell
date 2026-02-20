using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaInside.Shell;
using ShellExample.Models;

namespace ShellExample.ViewModels.ShopViewModels;

public class ProductCatalogFilterViewModel : ViewModelBase
{
	private readonly INavigator _navigationService;
	private string? _selectedCategory;

	public ICommand CloseCommand { get; }
	public ICommand ClearCommand { get; }

	public ProductCatalogFilterViewModel(INavigator navigationService)
	{
		_navigationService = navigationService;
		var items = DummyPlace.Products.Select(s => s.MainCategory)
			.Distinct()
			.OrderBy(o=>o);
		Categories = new ObservableCollection<string>();
		foreach (var item in items)
			Categories.Add(item);

		CloseCommand = new SimpleAsyncCommand(CloseAsync);
		ClearCommand = new SimpleAsyncCommand(ClearAsync);
	}

	private Task CloseAsync(CancellationToken cancellationToken)
	{
		return _navigationService.BackAsync(cancellationToken) ?? Task.CompletedTask;
	}

	private Task ClearAsync(CancellationToken cancellationToken)
	{
		return _navigationService.BackAsync(string.Empty, cancellationToken);
	}

	public ObservableCollection<string> Categories { get; }

	public string? SelectedCategory
	{
		get => _selectedCategory;
		set
		{
			SetField(ref _selectedCategory, value);
			_ = _navigationService.BackAsync(value);
		}
	}

	public void SetSelectedCategory(string? selected)
	{
		_selectedCategory = selected;
		OnPropertyChanged(nameof(SelectedCategory));
	}
}
