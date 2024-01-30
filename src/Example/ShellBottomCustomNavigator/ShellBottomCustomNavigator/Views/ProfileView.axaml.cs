using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellBottomCustomNavigator.Views;

public partial class ProfileView : Page
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

