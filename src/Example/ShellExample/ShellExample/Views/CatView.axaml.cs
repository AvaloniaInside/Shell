using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class CatView : UserControl, INavigation
{
	public CatView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-cat";
	public object? Title => "Cat";
	public object? Item { get; }
}

