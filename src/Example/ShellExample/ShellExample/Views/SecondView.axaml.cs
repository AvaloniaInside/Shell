using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellExample.Views;

public partial class SecondView : UserControl
{
	public SecondView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

