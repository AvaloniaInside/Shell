using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

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
	}

	public object? Header => "Home page";
}
