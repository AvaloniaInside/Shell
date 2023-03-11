using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class SettingView : UserControl, INavigation
{
	public SettingView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-gear";
	public object? Title => "Settings";
	public object? Item { get; }
}

