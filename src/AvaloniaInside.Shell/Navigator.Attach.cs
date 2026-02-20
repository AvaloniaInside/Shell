using Avalonia;
using Avalonia.Input;

namespace AvaloniaInside.Shell;

public partial class Navigator
{
	#region To Property

	public static readonly AttachedProperty<BindingNavigate> ToProperty =
		AvaloniaProperty.RegisterAttached<Navigator, AvaloniaObject, BindingNavigate>(
			"To",
			coerce: HandleToChanged);

	private static BindingNavigate HandleToChanged(AvaloniaObject obj, BindingNavigate value)
	{
		try
		{
			if (obj is ICommandSource commandSource)
			{
				((dynamic)commandSource).Command = value;
				value.Sender = obj;
			}
		}
		catch
		{
			/*IGNORE*/
		}

		return value;
	}

	public static BindingNavigate GetTo(AvaloniaObject element) =>
		element.GetValue(ToProperty);

	public static void SetTo(AvaloniaObject element, BindingNavigate parameter) =>
		element.SetValue(ToProperty, parameter);

	#endregion
}
