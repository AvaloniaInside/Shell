using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExample.Views;

public partial class SettingView : UserControl
{
	public SettingView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/user-solid.png";
}

