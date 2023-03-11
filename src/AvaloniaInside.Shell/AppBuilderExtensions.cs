using Avalonia;
using AvaloniaInside.Shell.Presenters;
using Splat;

namespace AvaloniaInside.Shell;

public static class AppBuilderExtensions
{
	public static AppBuilder UseShell(this AppBuilder builder) =>
		builder.AfterPlatformServicesSetup(_ =>
		{
			if (Locator.CurrentMutable is null)
			{
				return;
			}

			Locator.CurrentMutable.Register<INavigationRegistrar, NavigationRegistrar>();
			Locator.CurrentMutable.Register<IPresenterProvider, PresenterProvider>();
			Locator.CurrentMutable.Register<INavigationViewLocator, DefaultNavigationViewLocator>();
			Locator.CurrentMutable.Register<INavigationUpdateStrategy>(() =>
				new DefaultNavigationUpdateStrategy(Locator.Current.GetService<IPresenterProvider>()!));

			Locator.CurrentMutable.Register<INavigationService>(() =>
			{
				var registrar = Locator.Current.GetService<INavigationRegistrar>()!;
				return new NavigationService(
					registrar,
					new RelativeNavigateStrategy(registrar),
					Locator.Current.GetService<INavigationUpdateStrategy>()!,
					Locator.Current.GetService<INavigationViewLocator>()!);
			});
		});
}
