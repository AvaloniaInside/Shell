using System;
using Avalonia;
using AvaloniaInside.Shell.Presenters;

namespace AvaloniaInside.Shell;

public static class AppBuilderExtensions
{
	public static AppBuilder UseShell(this AppBuilder builder, Func<INavigationViewLocator>? viewLocatorFactory = null) =>
		builder.AfterPlatformServicesSetup(_ =>
		{
			AvaloniaLocator.CurrentMutable
				.Bind<INavigationRegistrar>().ToSingleton<NavigationRegistrar>()
				.Bind<IPresenterProvider>().ToSingleton<PresenterProvider>();

			if (viewLocatorFactory is null)
			{
				AvaloniaLocator.CurrentMutable
					.Bind<INavigationViewLocator>().ToSingleton<DefaultNavigationViewLocator>();
			}

			AvaloniaLocator.CurrentMutable
				.Bind<INavigationUpdateStrategy>()
				.ToFunc(() =>
					new DefaultNavigationUpdateStrategy(
						AvaloniaLocator.CurrentMutable.GetService<IPresenterProvider>()!));


			AvaloniaLocator.CurrentMutable
				.Bind<INavigator>()
				.ToFunc(() =>
				{
					var viewLocator = viewLocatorFactory != null ? viewLocatorFactory.Invoke() : AvaloniaLocator.CurrentMutable.GetService<INavigationViewLocator>()!;
					var registrar = AvaloniaLocator.CurrentMutable.GetService<INavigationRegistrar>()!;
					return new Navigator(
						registrar,
						new RelativeNavigateStrategy(registrar),
						AvaloniaLocator.CurrentMutable.GetService<INavigationUpdateStrategy>()!,
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
