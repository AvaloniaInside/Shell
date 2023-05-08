using AvaloniaInside.Shell;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        OpenCommand = ReactiveCommand.CreateFromTask(OpenAsync);
    }

    private Task OpenAsync(CancellationToken cancellationToken)
    {
        return _navigationService.NavigateAsync("/main", cancellationToken);
    }
}

