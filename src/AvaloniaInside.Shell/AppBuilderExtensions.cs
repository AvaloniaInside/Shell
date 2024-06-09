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

			if (viewLocatorFactory is null)
			{
				Locator.CurrentMutable.Register<INavigationViewLocator, DefaultNavigationViewLocator>();
			}
			
			Locator.CurrentMutable.Register<INavigationUpdateStrategy>(() =>
				new DefaultNavigationUpdateStrategy(Locator.Current.GetService<IPresenterProvider>()!));

			Locator.CurrentMutable.Register<INavigator>(() =>
			{
				var viewLocator = viewLocatorFactory != null ? viewLocatorFactory.Invoke() : Locator.Current.GetService<INavigationViewLocator>()!;
				var registrar = Locator.Current.GetService<INavigationRegistrar>()!;
				return new Navigator(
					registrar,
					new RelativeNavigateStrategy(registrar),
					Locator.Current.GetService<INavigationUpdateStrategy>()!,
					viewLocator
					);
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
