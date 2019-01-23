using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using AntRunner.Main.ViewModels;
using AntRunner.Models;

namespace AntRunner.Main.Views
{
    public partial class StartupWindow
    {
        public StartupWindow(FileSystemInfo map, IDictionary<AntProxy, AppDomain> ants, bool isDebug = false)
        {
            InitializeComponent();
            DataContext = new StartupViewModel(this, map, ants, isDebug);
            KeyUp += OnKeyUp;
        }

        //Window.InputBindings is not working! Use an event instead
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5) return;
            if (((StartupViewModel) DataContext).ToggleDebugCommand.CanExecute(null))
            {
                ((StartupViewModel)DataContext).ToggleDebugCommand.Execute(null);
            }
        }
    }
}
