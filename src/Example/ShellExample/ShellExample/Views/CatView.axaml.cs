using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExample.Views;

public partial class CatView : UserControl
{
	public CatView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/cat-solid.png";
}

