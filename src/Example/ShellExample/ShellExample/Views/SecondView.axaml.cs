using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

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

	public object? Header => "Second page";
	public object? Item { get; }
}

