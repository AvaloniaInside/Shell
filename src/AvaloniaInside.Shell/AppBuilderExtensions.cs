using System;
using Avalonia;
using AvaloniaInside.Shell.Presenters;
using Splat;

namespace AvaloniaInside.Shell;

public static class AppBuilderExtensions
{
	public static AppBuilder UseShell(this AppBuilder builder, Func<INavigationViewLocator>? viewLocatorFactory = null) =>
		builder.AfterPlatformServicesSetup(_ =>
		{
			if (Locator.CurrentMutable is null)
			{
				return;
			}

			Locator.CurrentMutable.Register<INavigationRegistrar, NavigationRegistrar>();
			Locator.CurrentMutable.Register<IPresenterProvider, PresenterProvider>();

			if (viewLocatorFactory != null)
			{
				Locator.CurrentMutable.Register(viewLocatorFactory, typeof(INavigationViewLocator));
			}
			else
			{
				Locator.CurrentMutable.Register<INavigationViewLocator, DefaultNavigationViewLocator>();
			}
			
			Locator.CurrentMutable.Register<INavigationUpdateStrategy>(() =>
				new DefaultNavigationUpdateStrategy(Locator.Current.GetService<IPresenterProvider>()!));

			Locator.CurrentMutable.Register<INavigator>(() =>
			{
				var registrar = Locator.Current.GetService<INavigationRegistrar>()!;
				return new Navigator(
					registrar,
					new RelativeNavigateStrategy(registrar),
					Locator.Current.GetService<INavigationUpdateStrategy>()!,
					Locator.Current.GetService<INavigationViewLocator>()!);
			});
		});

	public static AppBuilder UseShell(this AppBuilder builder, Func<NavigationNode, object> viewFactory)
		=> builder.UseShell(() => new DelegateNavigationViewLocator(viewFactory));

	private class DelegateNavigationViewLocator(Func<NavigationNode, object> viewFactory)
		    : INavigationViewLocator
	    {
		    public object GetView(NavigationNode navigationItem)
		    {
			    return viewFactory(navigationItem);
		    }
	    }
}
