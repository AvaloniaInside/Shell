using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExample.Views.Widgets;

public partial class UserProfileWidgetView : UserControl
{
	public UserProfileWidgetView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

