using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellBottomCustomNavigator.Views;

public partial class SimpleDialog : Page
{
	public SimpleDialog()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	private void OkButton_OnClick(object? sender, RoutedEventArgs e)
	{
		Navigator?.BackAsync();
	}

	private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
	{
        Navigator?.BackAsync();
	}
}

