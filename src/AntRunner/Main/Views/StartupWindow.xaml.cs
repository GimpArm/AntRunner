using System.Collections.Generic;
using System.IO;
using AntRunner.Interface;
using AntRunner.Main.ViewModels;

namespace AntRunner.Main.Views
{
    public partial class StartupWindow
    {
        public StartupWindow(FileSystemInfo map, IEnumerable<Ant> ants, bool isDebug = false)
        {
            InitializeComponent();
            DataContext = new StartupViewModel(this, map, ants, isDebug);
        }
    }
}
