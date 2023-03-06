using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using AvaloniaInside.Shell;
using ReactiveUI;
using ShellExample.Models;

namespace ShellExample.ViewModels.ShopViewModels;

public class ProductCatalogFilterViewModel : ViewModelBase
{
	private string? _selectedCategory;

	public ICommand CloseCommand { get; }
	public ICommand ClearCommand { get; }

	public ProductCatalogFilterViewModel()
	{
		var items = DummyPlace.Products.Select(s => s.MainCategory)
			.Distinct()
			.Order();
		Categories = new ObservableCollection<string>(items);

		CloseCommand = ReactiveCommand.CreateFromTask(CloseAsync);
		ClearCommand = ReactiveCommand.CreateFromTask(ClearAsync);
	}

	private Task CloseAsync(CancellationToken cancellationToken)
	{
		return AvaloniaLocator.CurrentMutable
			.GetService<INavigationService>()?
			.BackAsync(cancellationToken) ?? Task.CompletedTask;
	}

	private Task ClearAsync(CancellationToken cancellationToken)
	{
		return AvaloniaLocator.CurrentMutable
			.GetService<INavigationService>()?
			.BackAsync(string.Empty, cancellationToken) ?? Task.CompletedTask;
	}

	public ObservableCollection<string> Categories { get; }

	public string? SelectedCategory
	{
		get => _selectedCategory;
		set
		{
			this.RaiseAndSetIfChanged(ref _selectedCategory, value);
			_ = AvaloniaLocator.CurrentMutable
				.GetService<INavigationService>()?
				.BackAsync(value) ?? Task.CompletedTask;
		}
	}

	public void SetSelectedCategory(string? selected)
	{
		_selectedCategory = selected;
		this.RaisePropertyChanged(nameof(SelectedCategory));
	}
}
