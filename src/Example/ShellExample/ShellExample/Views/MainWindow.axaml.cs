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

		if (MainView.ContentView != null)
		{
			MainView.ContentView.PropertyChanged += (sender, args) =>
			{
				if (args.Property == StackContentView.CurrentViewProperty)
				{
					if (args.NewValue is AvaloniaObject newValue)
					{
						var header = NavigationBar.GetHeader(newValue) as string;
						Title = header ?? "Shell Example";
						System.Diagnostics.Debug.WriteLine($"Window title updated to: {Title}");
					}
				}
			};
		}
	}
}
