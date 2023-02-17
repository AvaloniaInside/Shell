using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using AvaloniaInside.Shell.Presenters;
using ShellExample.ViewModels;
using ShellExample.Views;

namespace ShellExample;

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);

		AvaloniaLocator.CurrentMutable.Bind<IPresenterProvider>().ToSingleton<PresenterProvider>();
		AvaloniaLocator.CurrentMutable.Bind<INavigationViewLocator>().ToSingleton<DefaultNavigationViewLocator>();
		AvaloniaLocator.CurrentMutable.Bind<INavigateStrategy>().ToSingleton<RelativeNavigateStrategy>();
		AvaloniaLocator.CurrentMutable.Bind<INavigationUpdateStrategy>().ToConstant(
			new DefaultNavigationUpdateStrategy(AvaloniaLocator.Current.GetService<IPresenterProvider>()));

		AvaloniaLocator.CurrentMutable.Bind<INavigationService>().ToConstant(
			new NavigationService(
				AvaloniaLocator.Current.GetService<INavigateStrategy>(),
				AvaloniaLocator.Current.GetService<INavigationUpdateStrategy>(),
				AvaloniaLocator.Current.GetService<INavigationViewLocator>()));

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
