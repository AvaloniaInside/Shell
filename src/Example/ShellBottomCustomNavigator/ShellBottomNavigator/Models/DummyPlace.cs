using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ShellBottomNavigator.Models;

public static class DummyPlace
{
	public static ProductDto[] Products { get; }

	public static string GetProductsJson()
	{
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream("ShellBottomNavigator.Resources.Products.json");
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}

	static DummyPlace()
	{
		Products = JsonConvert.DeserializeObject<ProductDto[]>(GetProductsJson()) ?? throw new Exception("Where is my resource?!");
	}
}
