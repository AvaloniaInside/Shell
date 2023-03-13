using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class HomePage : UserControl, INavigation
{
	public HomePage()
	{
		InitializeComponent();
		DataContext = new ViewModels.HomePageViewModel(MainView.Current.ShellViewMain.Navigation);
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/house-solid.png";

	public object? Title => "Home";

	public object? Item { get; } = new Button()
	{
		Content = "Add"
	};
}
