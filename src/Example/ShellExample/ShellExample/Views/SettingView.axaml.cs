using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ShellExample.ViewModels;

namespace ShellExample.Views;

public partial class SettingView : UserControl
{
	public SettingView()
	{
		InitializeComponent();
		DataContext = new SettingViewModel()
		{
			MainViewModel = (MainViewModel)MainView.Current.DataContext
		};

    }

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/user-solid.png";
}

