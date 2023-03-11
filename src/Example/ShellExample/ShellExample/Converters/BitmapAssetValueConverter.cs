using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

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
			Uri uri;

			// Allow for assembly overrides
			if (rawUri.StartsWith("avares://"))
			{
				uri = new Uri(rawUri);
			}
			else
			{
				string assemblyName = GetType().Assembly.GetName().Name;
				uri = new Uri($"avares://{assemblyName}/{rawUri.TrimStart('/')}");
			}

			var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
			var asset = assets.Open(uri);

			return new Bitmap(asset);
		}

		throw new NotSupportedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotSupportedException();
	}
}
