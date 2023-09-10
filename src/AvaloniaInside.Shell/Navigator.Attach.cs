using Avalonia;
using Avalonia.Input;
using System;

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
}
