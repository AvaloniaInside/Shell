using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExample.Views;

public partial class DogView : UserControl
{
	public DogView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/dog-solid.png";
}

