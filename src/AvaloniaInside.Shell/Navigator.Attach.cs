using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace AvaloniaInside.Shell
{
    public partial class Navigator
    {
        static Navigator()
        {
            ToProperty.Changed.Subscribe(HandleToChanged);
        }

        #region To Property

        public static readonly AttachedProperty<BindingNavigate> ToProperty =
            AvaloniaProperty.RegisterAttached<Navigator, AvaloniaObject, BindingNavigate>("To");

        private static void HandleToChanged(AvaloniaPropertyChangedEventArgs<BindingNavigate> e)
        {
            try
            {
                if (e.Sender is ICommandSource commandSource)
                {
                    if (e.NewValue is { HasValue: true } v)
                    {
                        ((dynamic)commandSource).Command = v.Value;
                        v.Value.Sender = e.Sender;
                    }
                }
            }
            catch { /*IGNORE*/ }
        }

        public static BindingNavigate GetTo(AvaloniaObject element) =>
            element.GetValue(ToProperty);

        public static void SetTo(AvaloniaObject element, BindingNavigate parameter) =>
            element.SetValue(ToProperty, parameter);

        #endregion
    }

    [TypeConverter(typeof(BindingNavigateConverter))]
    public class BindingNavigate : AvaloniaObject, ICommand
    {
        // Temporary solution for now, Can execute should refer to current shell
        private static bool _singletonCanExecute = true;
        private static EventHandler? _singletonCanExecuteChanged;

        public AvaloniaObject? Sender { get; internal set; }
        public string Path { get; set; }

        public event EventHandler? CanExecuteChanged
        {
            add => _singletonCanExecuteChanged += value;
            remove => _singletonCanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter) => _singletonCanExecute;
        public void Execute(object? parameter) => ExecuteAsync(parameter, CancellationToken.None);

        public async Task ExecuteAsync(object? parameter, CancellationToken cancellationToken)
        {
            if (Sender is not Visual visual) return;
            if (visual.FindAncestorOfType<ShellView>() is not { } shell) return;

            _singletonCanExecute = false;
            _singletonCanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try
            {
                if (parameter != null)
                    await shell.Navigator.NavigateAsync(Path, parameter, cancellationToken);
                else
                    await shell.Navigator.NavigateAsync(Path, cancellationToken);
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
