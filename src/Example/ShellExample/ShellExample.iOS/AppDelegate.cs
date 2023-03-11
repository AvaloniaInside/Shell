using Foundation;
using Avalonia;
using Avalonia.iOS;
using Avalonia.ReactiveUI;
using AvaloniaInside.Shell;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace ShellExample.iOS;

// The UIApplicationDelegate for the application. This class is responsible for launching the
// User Interface of the application, as well as listening (and optionally responding) to
// application events from iOS.
[Register("AppDelegate")]
public partial class AppDelegate : AvaloniaAppDelegate<App>
{
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		return builder.UseReactiveUI()
			.UseShell()
			.WithIcons(container => container
				.Register<FontAwesomeIconProvider>());
	}
}
