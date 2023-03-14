using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShellExample.Helpers;

namespace ShellExample.Converters;

public class BitmapAssetValueConverter : IValueConverter
{
	public static BitmapAssetValueConverter Instance = new BitmapAssetValueConverter();

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value == null)
			return null;

		if (value is string rawUri && targetType.IsAssignableFrom(typeof(Bitmap)))
		{
			return rawUri.GetBitmapFromAssets();
		}

		throw new NotSupportedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
