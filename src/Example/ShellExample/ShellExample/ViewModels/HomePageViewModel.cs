using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaInside.Shell;
using ReactiveUI;

namespace ShellExample.ViewModels;

public class HomePageViewModel : ViewModelBase
{
	private readonly INavigator _navigationService;

	public ICommand NavigateToSecondPage { get; set; }
	public ICommand ShowDialogCommand { get; set; }
	public ICommand ToSecondAndPutQueryDataCommand { get; set; }

	public HomePageViewModel(INavigator navigationService)
	{
		_navigationService = navigationService;
		NavigateToSecondPage = ReactiveCommand.CreateFromTask(Navigate);
		ShowDialogCommand = ReactiveCommand.CreateFromTask(ShowDialog);
		ToSecondAndPutQueryDataCommand = ReactiveCommand.CreateFromTask(ToSecondAndPutQueryData);
	}

	private Task ShowDialog(CancellationToken cancellationToken)
	{
		return _navigationService.NavigateAsync("/main/home/confirmation", cancellationToken);
	}

	private Task Navigate()
	{
		return _navigationService.NavigateAsync("/second");
	}
	
	private Task ToSecondAndPutQueryData()
	{
		return _navigationService.NavigateAsync("/second",new
		{
			Name = "Shell Example"
		});
	}
}
