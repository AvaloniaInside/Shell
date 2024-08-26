using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaInside.Shell;

public class BindingNavigateConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        return value is string s ? new BindingNavigate { Path = s } : null;
    }
}