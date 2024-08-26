using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ShellBottomNavigator.Views.Widgets;

public partial class CalendarWidgetView : UserControl
{
	public string JustNow => DateTime.Now.ToString("dd MMMM yyyy");

	public CalendarWidgetView()
	{
		InitializeComponent();
		DataContext = this;
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

