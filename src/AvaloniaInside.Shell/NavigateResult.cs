namespace AvaloniaInside.Shell;

public class NavigateResult
{
	internal NavigateResult(bool hasArgument, object? argument)
	{
		HasArgument = hasArgument;
		Argument = argument;
	}

	public bool HasArgument { get; }
	public object? Argument { get; }

	public T? As<T>() => HasArgument && Argument is T argument ? argument : default;
	
	public static NavigateResult FromResult<T>(T argument) => 	new(true, argument);

	public static readonly NavigateResult Completed = new NavigateResult(false, null);
}


