using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using ShellExample.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace ShellExample.Views;

public partial class SettingView : Page
{
	public SettingView()
	{
		InitializeComponent();
	}

	public override Task InitialiseAsync(CancellationToken cancellationToken)
	{
        DataContext = new SettingViewModel()
        {
            MainViewModel = (MainViewModel)MainView.Current.DataContext
        };

        return base.InitialiseAsync(cancellationToken);
	}

    private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

