using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class NavigationCancellationDemoPage : Page
{
    public NavigationCancellationDemoPage()
    {
        InitializeComponent();
    }

    public override async Task InitialiseAsync(CancellationToken cancellationToken)
    {
        // This page doesn't cancel its own navigation - it's just a demo page
        await base.InitialiseAsync(cancellationToken);
    }

    private async void NavigateClick(object? sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        try
        {
            button.IsEnabled = false;
            await Navigator.NavigateAsync("/navigate-cancelled");
            button.Content = "✅ Successfully cancelled!";


            await Task.Delay(2000);

            button.Content = "Test NavigateAsync Cancellation";
        }
        finally
        {
            button.IsEnabled = true;
        }
    }


    private async void NavigateAndWaitClick(object? sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        try
        {
            button.IsEnabled = false;
            try
            {
                await Navigator.NavigateAndWaitAsync("/navigate-cancelled");
            }
            catch (OperationCanceledException)
            {
                button.Content = "✅ Successfully cancelled!";
            }

            await Task.Delay(2000);

            button.Content = "Test NavigateAndWaitAsync Cancellation";
        }
        finally
        {
            button.IsEnabled = true;
        }
    }
}