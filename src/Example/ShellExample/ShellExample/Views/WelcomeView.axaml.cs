using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaInside.Shell;
using Projektanker.Icons.Avalonia;
using System.Threading;
using System.Threading.Tasks;

namespace ShellExample.Views
{
    public partial class WelcomeView : Page
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        public override Task InitialiseAsync(CancellationToken cancellationToken)
        {
            DataContext = new ViewModels.WelcomeViewModel(Navigator);
            return base.InitialiseAsync(cancellationToken);
        }
    }
}
