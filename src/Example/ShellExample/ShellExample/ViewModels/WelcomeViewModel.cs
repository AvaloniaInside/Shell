using AvaloniaInside.Shell;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShellExample.ViewModels;

internal class WelcomeViewModel
{
    private readonly INavigator _navigationService;
    public ICommand OpenCommand { get; set; }

    public WelcomeViewModel(INavigator navigationService)
    {
        _navigationService = navigationService;
        OpenCommand = new SimpleAsyncCommand(OpenAsync);
    }

    private Task OpenAsync(CancellationToken cancellationToken)
    {
        return _navigationService.NavigateAsync("/main", cancellationToken);
    }
}

