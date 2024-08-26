using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using ShellBottomNavigator.Helpers;

namespace ShellBottomNavigator.Converters;

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
