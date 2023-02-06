using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell.Services;
using AvaloniaInside.Shell.Views;

namespace ShellExample.Views;

public partial class HomePage : UserControl, INavigation
{
	public HomePage()
	{
		InitializeComponent();
		DataContext = new ViewModels.HomePageViewModel(AvaloniaLocator.CurrentMutable.GetService<INavigationService>());
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);

		var navigationService = AvaloniaLocator.CurrentMutable.GetService<INavigationService>();

		navigationService.RegisterRoute("/home", typeof(HomePage), NavigationNodeType.Page);
		navigationService.RegisterRoute("/second", typeof(SecondView), NavigationNodeType.Page);
	}

	public object Header => "Home page";
}
