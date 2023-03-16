using System.Runtime.Versioning;
using Avalonia;
using Avalonia.ReactiveUI;
using ShellExample;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]

internal partial class Program
{
	private static void Main(string[] args) => BuildAvaloniaApp()
		.UseReactiveUI()
		.StartBrowserAppAsync("out");

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder.Configure<App>();
}
