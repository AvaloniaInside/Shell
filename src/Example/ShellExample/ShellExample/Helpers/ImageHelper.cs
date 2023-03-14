using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ShellExample.Helpers;

public static class ImageHelper
{
	public static Bitmap? GetBitmapFromAssets(this string rawUri)
	{
		Uri uri;

		// Allow for assembly overrides
		if (rawUri.StartsWith("avares://"))
		{
			uri = new Uri(rawUri);
		}
		else
		{
			var assemblyName = typeof(ImageHelper).Assembly.GetName().Name;
			uri = new Uri($"avares://{assemblyName}/{rawUri.TrimStart('/')}");
		}

		var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
		var asset = assets.Open(uri);

		return new Bitmap(asset);
	}

}
