using Avalonia.Controls;

namespace ShellExample.Views
{
    public partial class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
            DataContext = new ViewModels.WelcomeViewModel(MainView.Current.ShellViewMain.Navigator);
        }
    }
}
