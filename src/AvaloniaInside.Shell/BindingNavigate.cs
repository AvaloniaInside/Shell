using Avalonia;
using Avalonia.Animation;
using Avalonia.VisualTree;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaInside.Shell
{
    [TypeConverter(typeof(BindingNavigateConverter))]
    public class BindingNavigate : AvaloniaObject, ICommand
    {
        // Temporary solution for now, Can execute should refer to current shell
        private static bool _singletonCanExecute = true;
        private static EventHandler? _singletonCanExecuteChanged;

        public AvaloniaObject? Sender { get; internal set; }
        public string Path { get; set; }
        public NavigateType? Type { get; set; }
        public IPageTransition? Transition { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add => _singletonCanExecuteChanged += value;
            remove => _singletonCanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter) => _singletonCanExecute;
        public void Execute(object? parameter) => ExecuteAsync(parameter, CancellationToken.None).Wait();

        public async Task ExecuteAsync(object? parameter, CancellationToken cancellationToken)
        {
            if (Sender is not Visual visual) return;
            if (visual.FindAncestorOfType<ShellView>() is not { } shell) return;

            _singletonCanExecute = false;
            _singletonCanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try
            {
                if (parameter != null)
                    await shell.Navigator.NavigateAsync(
                        Path, 
                        Type, 
                        parameter, 
                        Sender, 
                        true, 
                        Transition, 
                        cancellationToken);
                else
                    await shell.Navigator.NavigateAsync(
                        Path, 
                        Type, 
                        Sender, 
                        true, 
                        Transition, 
                        cancellationToken);
            }
            finally
            {
                _singletonCanExecute = true;
                _singletonCanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static implicit operator BindingNavigate(string path) => new BindingNavigate
        {
            Path = path
        };
    }
}
