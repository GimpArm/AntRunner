using System;
using System.Linq;
using System.Windows;
using AntRunner.Main.Views;

namespace AntRunner
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var main = new StartupWindow(e.Args.Any(x => x.Equals("debug", StringComparison.InvariantCultureIgnoreCase)));
            main.Show();
        }
    }
}
