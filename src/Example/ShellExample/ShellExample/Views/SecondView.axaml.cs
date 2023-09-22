using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaInside.Shell;
using ShellExample.ViewModels;

namespace ShellExample.Views;

public partial class SecondView : Page
{
	public SecondView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override Task InitialiseAsync(CancellationToken cancellationToken)
	{
		DataContext = new SecondViewModel();
		return base.InitialiseAsync(cancellationToken);
	}
}

