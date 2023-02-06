using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell.Views;

namespace ShellExample.Views;

public partial class SecondView : UserControl, INavigation
{
	public SecondView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public object Header => "Second page";
}

