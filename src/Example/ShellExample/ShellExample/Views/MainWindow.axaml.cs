using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class MainWindow : Window
{
	public MainWindow()
	{
		this.AttachDevTools();
		InitializeComponent();
	}

	protected override void OnLoaded(RoutedEventArgs e)
	{
		base.OnLoaded(e);

		// Subscribe to navigation events to update window title
		if (MainView.Navigator != null)
		{
			MainView.Navigator.OnNavigate += OnNavigate;
		}
	}

	protected override void OnUnloaded(RoutedEventArgs e)
	{
		// Unsubscribe from navigation events
		if (MainView.Navigator != null)
		{
			MainView.Navigator.OnNavigate -= OnNavigate;
		}

		base.OnUnloaded(e);
	}

	private void OnNavigate(object? sender, NaviagateEventArgs e)
	{
		if (e.To is AvaloniaObject targetView)
		{
			var header = NavigationBar.GetHeader(targetView) as string;
			Title = header ?? "Shell Example";
		}
	}
}
