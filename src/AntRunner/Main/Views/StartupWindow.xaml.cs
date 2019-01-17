using System;
using System.Collections.Generic;
using System.IO;
using AntRunner.Interface;
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
        }
    }
}
