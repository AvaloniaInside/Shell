using Android.App;
using Android.Content;
using Android.OS;
using Application = Android.App.Application;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace ShellExample.Android;

[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
public class SplashActivity : AvaloniaSplashActivity<App>
{
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		return base.CustomizeAppBuilder(builder)
			.UseReactiveUI()
			.WithIcons(container => container
				.Register<FontAwesomeIconProvider>());
	}

	protected override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
	}

	protected override void OnResume()
	{
		base.OnResume();

		StartActivity(new Intent(Application.Context, typeof(MainActivity)));
	}
}
