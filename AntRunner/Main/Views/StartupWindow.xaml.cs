using AntRunner.Main.ViewModels;

namespace AntRunner.Main.Views
{
    public partial class StartupWindow
    {
        public StartupWindow(bool isDebug = false)
        {
            InitializeComponent();
            DataContext = new StartupViewModel(this, isDebug);
        }
    }
}
