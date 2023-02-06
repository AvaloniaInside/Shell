using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell.Views;

namespace ShellExample.Views;

public partial class HomePage : UserControl, INavigation
{
	public HomePage()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public object Header => "Home page";
}

