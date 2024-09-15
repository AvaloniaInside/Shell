using Avalonia.Interactivity;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class MainView : ShellView
{
	public static MainView Current { get; private set; }

	public MainView()
	{
		Current = this;
		InitializeComponent();
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		Current = this;
		base.OnLoaded(e);
	}
}
