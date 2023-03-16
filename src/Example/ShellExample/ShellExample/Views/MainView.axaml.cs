using Avalonia.Controls;

namespace ShellExample.Views;

public partial class MainView : UserControl
{
	public static MainView Current { get; private set; }

	public MainView()
	{
		Current = this;
		InitializeComponent();
	}
	protected override void OnLoaded()
	{
		Current = this;
		base.OnLoaded();
	}
}
