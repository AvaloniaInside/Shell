using AvaloniaInside.Shell.Query;
using ReactiveUI;

namespace ShellExample.ViewModels;

[QueryProperty(nameof(Name))]
public class SecondViewModel : ViewModelBase, IQueryAttributable
{
    
    private string _name;

    public string Name
    {
        get => _name;
        set { this.RaiseAndSetIfChanged(ref _name, value); }
    }

    public void ApplyQueryAttributes(object? paras)
    {
    }
}