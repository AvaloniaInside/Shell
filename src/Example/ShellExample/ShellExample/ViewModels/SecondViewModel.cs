using System.Collections.Generic;
using AvaloniaInside.Shell;
using ReactiveUI;

namespace ShellExample.ViewModels;

public class SecondViewModel : ViewModelBase, IQueryAttributable
{
    private string _name;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        query.TryGetValue(nameof(Name),out var nameValue);
        Name = nameValue?.ToString();
    }
}