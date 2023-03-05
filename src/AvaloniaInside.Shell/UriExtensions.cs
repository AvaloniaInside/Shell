using System;

namespace AvaloniaInside.Shell;

public static class UriExtensions
{
	public static string GetParentPath(this Uri uri)
	{
		var finalPath = uri.AbsolutePath.EndsWith("/")
			? (new Uri(uri, "..")).AbsolutePath.TrimEnd('/')
			: (new Uri(uri, ".")).AbsolutePath.TrimEnd('/');

		return finalPath.Length > 0 ? finalPath : "/";
	}
}
