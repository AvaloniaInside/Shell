using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class SimpleDialog : UserControl
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
		MainView.Current.ShellViewMain.Navigation.BackAsync();
	}

	private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
	{
		MainView.Current.ShellViewMain.Navigation.BackAsync();
	}
}

