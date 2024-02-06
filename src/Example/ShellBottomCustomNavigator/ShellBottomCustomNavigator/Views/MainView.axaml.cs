using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ShellBottomCustomNavigator.Views;

public partial class MainView : UserControl
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