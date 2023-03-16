using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace ShellExample.Views;

public partial class PetsTabControlView : TabControl, IStyleable
{
	public Type StyleKey => typeof(TabControl);

	public PetsTabControlView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "/Assets/Icons/paw-solid.png";
}

