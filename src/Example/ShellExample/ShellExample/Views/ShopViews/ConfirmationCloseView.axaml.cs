using AvaloniaInside.Shell;

namespace ShellExample.Views.ShopViews;

public partial class ConfirmationCloseView : Page
{
    public ConfirmationCloseView()
    {
        InitializeComponent();
    }

    private void NoClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Navigator.BackAsync(false);
    }

    private void YesClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Navigator.BackAsync(true);
    }
}