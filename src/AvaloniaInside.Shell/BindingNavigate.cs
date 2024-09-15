using Avalonia;
using Avalonia.VisualTree;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaInside.Shell;

[TypeConverter(typeof(BindingNavigateConverter))]
public class BindingNavigate : AvaloniaObject, ICommand
{
	private bool _canExecute = true;
	private EventHandler? _canExecuteChanged;

	public AvaloniaObject? Sender { get; internal set; }
	public required string Path { get; set; }
	public NavigateType? Type { get; set; }

	public event EventHandler? CanExecuteChanged
	{
		add => _canExecuteChanged += value;
		remove => _canExecuteChanged -= value;
	}

	public bool CanExecute(object? parameter) => _canExecute;
	public void Execute(object? parameter) => _ = ExecuteAsync(parameter, CancellationToken.None);

	public async Task ExecuteAsync(object? parameter, CancellationToken cancellationToken)
	{
		if (Sender is not Visual visual) return;
		if (visual.FindAncestorOfType<ShellView>() is not { } shell) return;

		_canExecute = false;
		_canExecuteChanged?.Invoke(this, EventArgs.Empty);
		try
		{
			if (parameter != null)
				await shell.Navigator.NavigateAsync(
					Path,
					Type,
					parameter,
					Sender,
					true,
					cancellationToken);
			else
				await shell.Navigator.NavigateAsync(
					Path,
					Type,
					Sender,
					true,
					cancellationToken);
		}
		finally
		{
			_canExecute = true;
			_canExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public static implicit operator BindingNavigate(string path) => new BindingNavigate
	{
		Path = path
	};
}
