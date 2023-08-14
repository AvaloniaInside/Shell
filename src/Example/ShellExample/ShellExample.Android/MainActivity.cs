using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using AvaloniaInside.Shell;

namespace ShellExample.Android;

[Activity(Label = "ShellExample.Android", Theme = "@style/MyTheme.Main", Icon = "@drawable/icon",

	LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            //.WithInterFont()
            .UseReactiveUI()
            .UseShell();
    }
}
