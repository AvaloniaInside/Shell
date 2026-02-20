using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvaloniaInside.Shell;

public class SimpleAsyncCommand : ICommand
{
	private readonly Func<object?, CancellationToken, Task> _execute;
	private readonly Func<object?, bool>? _canExecute;
	private bool _isExecuting;

	public SimpleAsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
		: this((_, _) => execute(), canExecute == null ? null : _ => canExecute())
	{
	}

	public SimpleAsyncCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute = null)
		: this((param, _) => execute(param), canExecute)
	{
	}

	public SimpleAsyncCommand(Func<CancellationToken, Task> execute, Func<bool>? canExecute = null)
		: this((_, ct) => execute(ct), canExecute == null ? null : _ => canExecute())
	{
	}

	public SimpleAsyncCommand(Func<object?, CancellationToken, Task> execute, Func<object?, bool>? canExecute = null)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute;
	}

	public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

	public void Execute(object? parameter)
	{
		// Fire-and-forget from ICommand.Execute (void). Use a safe, observed task.
		_ = ExecuteAsync(parameter, CancellationToken.None);
	}

	public event EventHandler? CanExecuteChanged;

	public async Task ExecuteAsync(object? parameter, CancellationToken cancellationToken)
	{
		if (_isExecuting) return;

		try
		{
			_isExecuting = true;
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);

			await _execute(parameter, cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			_isExecuting = false;
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
