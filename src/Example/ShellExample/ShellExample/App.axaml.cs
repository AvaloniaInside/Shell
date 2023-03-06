using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using AvaloniaInside.Shell.Presenters;
using ShellExample.ViewModels;
using ShellExample.Views;
using ShellExample.Views.ShopViews;

namespace ShellExample;

public partial class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);

		AvaloniaLocator.CurrentMutable.Bind<INavigationRegistrar>().ToSingleton<NavigationRegistrar>();
		AvaloniaLocator.CurrentMutable.Bind<IPresenterProvider>().ToSingleton<PresenterProvider>();
		AvaloniaLocator.CurrentMutable.Bind<INavigationViewLocator>().ToSingleton<DefaultNavigationViewLocator>();
		AvaloniaLocator.CurrentMutable.Bind<INavigateStrategy>().ToConstant(
			new RelativeNavigateStrategy(AvaloniaLocator.Current.GetService<INavigationRegistrar>()));
		AvaloniaLocator.CurrentMutable.Bind<INavigationUpdateStrategy>().ToConstant(
			new DefaultNavigationUpdateStrategy(AvaloniaLocator.Current.GetService<IPresenterProvider>()));

		AvaloniaLocator.CurrentMutable.Bind<INavigationService>().ToConstant(
			new NavigationService(
				AvaloniaLocator.Current.GetService<INavigationRegistrar>(),
				AvaloniaLocator.Current.GetService<INavigateStrategy>(),
				AvaloniaLocator.Current.GetService<INavigationUpdateStrategy>(),
				AvaloniaLocator.Current.GetService<INavigationViewLocator>()));




		var navigationService = AvaloniaLocator.CurrentMutable.GetService<INavigationService>();

		navigationService.RegisterHost("/main", typeof(MainTabControl), "/main/home", NavigateType.Normal);
		navigationService.RegisterPage("/main/home", typeof(HomePage), NavigateType.Normal);
		navigationService.RegisterHost("/main/pets", typeof(PetsTabControlView), "/main/pets/cat", NavigateType.Normal);
		navigationService.RegisterPage("/main/pets/cat", typeof(CatView), NavigateType.Normal);
		navigationService.RegisterPage("/main/pets/dog", typeof(DogView), NavigateType.Normal);
		navigationService.RegisterPage("/main/product", typeof(ProductCatalogView), NavigateType.Normal);
		navigationService.RegisterPage("/main/product/filter", typeof(ProductCatalogFilterView), NavigateType.Modal);
		navigationService.RegisterPage("/main/setting", typeof(SettingView), NavigateType.Normal);
		navigationService.RegisterPage("/main/setting/profile", typeof(ProfileView), NavigateType.Normal);

		navigationService.RegisterPage("/main/home/confirmation", typeof(SimpleDialog), NavigateType.Modal);

		navigationService.RegisterPage("/second", typeof(SecondView), NavigateType.Normal);
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
