using AvaloniaInside.Shell;
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
