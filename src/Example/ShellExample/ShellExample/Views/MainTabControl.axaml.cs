using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace ShellExample.Views;

public partial class MainTabControl : TabControl, IStyleable
{
	public Type StyleKey => typeof(TabControl);

	public MainTabControl()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

