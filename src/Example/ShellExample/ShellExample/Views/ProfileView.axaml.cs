using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExample.Views;

public partial class ProfileView : UserControl
{
	public ProfileView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-user";
}

