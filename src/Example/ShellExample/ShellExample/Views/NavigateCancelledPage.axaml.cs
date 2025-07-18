using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaInside.Shell;

namespace ShellExample.Views;

public partial class NavigateCancelledPage : Page
{
    public NavigateCancelledPage()
    {
        InitializeComponent();
    }

    public override async Task InitialiseAsync(CancellationToken cancellationToken)
    {
        Debug.WriteLine("NavigateCancelledPage.InitialiseAsync: Starting initialization");
        
        try
        {
            // This demonstrates cancelling the current navigation during InitializeAsync
            // The navigation should be cancelled and the user should go back to the previous page
            if (Navigator != null)
            {
                Debug.WriteLine("NavigateCancelledPage.InitialiseAsync: Calling Navigator.BackAsync to cancel navigation");
                await Navigator.BackAsync();
                Debug.WriteLine("NavigateCancelledPage.InitialiseAsync: Navigation cancelled successfully");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"NavigateCancelledPage.InitialiseAsync: Exception occurred: {ex.Message}");
            throw;
        }
        
        await base.InitialiseAsync(cancellationToken);
        Debug.WriteLine("NavigateCancelledPage.InitialiseAsync: Completed");
    }

    public override async Task AppearAsync(CancellationToken cancellationToken)
    {
        Debug.WriteLine("NavigateCancelledPage.AppearAsync: This should not be called if navigation is cancelled");
        await base.AppearAsync(cancellationToken);
    }
}