using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaInside.Shell;
using DynamicData;
using ReactiveUI;
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
		Categories.AddRange(items);

		CloseCommand = ReactiveCommand.CreateFromTask(CloseAsync);
		ClearCommand = ReactiveCommand.CreateFromTask(ClearAsync);
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
			this.RaiseAndSetIfChanged(ref _selectedCategory, value);
			_ = _navigationService.BackAsync(value);
		}
	}

	public void SetSelectedCategory(string? selected)
	{
		_selectedCategory = selected;
		this.RaisePropertyChanged(nameof(SelectedCategory));
	}
}
