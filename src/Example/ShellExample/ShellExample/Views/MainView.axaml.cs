using Avalonia;
using Avalonia.Controls;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class MainView : UserControl
{
	public MainView()
	{
		var navigationService = AvaloniaLocator.CurrentMutable.GetService<INavigationService>();

		navigationService.RegisterRoute("/home", typeof(HomePage), NavigationNodeType.Page, NavigateType.Normal);
		navigationService.RegisterRoute("/home/confirmation", typeof(SimpleDialog), NavigationNodeType.Page, NavigateType.Modal);

		navigationService.RegisterRoute("/second", typeof(SecondView), NavigationNodeType.Page, NavigateType.Normal);

		InitializeComponent();
	}
}
