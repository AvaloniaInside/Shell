using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class HomePage : UserControl, INavigation
{
	public HomePage()
	{
		InitializeComponent();
		DataContext = new ViewModels.HomePageViewModel(MainView.Current.ShellViewMain.Navigation);
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-house";

	public object? Title => "Home";

	public object? Item { get; } = new Button()
	{
		Content = "Add"
	};
}
