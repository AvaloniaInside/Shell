using System.Threading;
using System.Threading.Tasks;
using AvaloniaInside.Shell;
using ShellBottomCustomNavigator.ViewModels;

namespace ShellBottomCustomNavigator.Views
{
    public partial class WelcomeView : Page
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        public override Task InitialiseAsync(CancellationToken cancellationToken)
        {
            DataContext = new WelcomeViewModel(Navigator);
            return base.InitialiseAsync(cancellationToken);
        }
    }
}
