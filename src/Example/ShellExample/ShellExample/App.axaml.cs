using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell.Services;
using ShellExample.ViewModels;
using ShellExample.Views;

namespace ShellExample;

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
		AvaloniaLocator.CurrentMutable.Bind<INavigationService>().ToSingleton<NavigationService>();
		AvaloniaLocator.CurrentMutable.Bind<INavigationViewLocator>().ToSingleton<DefaultNavigationViewLocator>();
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow
			{
				DataContext = new MainViewModel()
			};
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
		{
			singleViewPlatform.MainView = new MainView
			{
				DataContext = new MainViewModel()
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}
