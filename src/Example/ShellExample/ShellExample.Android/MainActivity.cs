using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using Avalonia.Android;

namespace ShellExample.Android;

[Activity(Label = "ShellExample.Android", Theme = "@style/MyTheme.Main", Icon = "@drawable/icon",

	LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
public class MainActivity : AvaloniaMainActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
	}
}
