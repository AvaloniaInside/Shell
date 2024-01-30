using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellBottomCustomNavigator.Views.Widgets;

public partial class WeatherView : UserControl
{
	public WeatherView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

