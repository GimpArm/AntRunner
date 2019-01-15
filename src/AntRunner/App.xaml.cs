using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AntRunner.Interface;
using AntRunner.Main.Views;
using AntRunner.Models;

namespace AntRunner
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var isDebug = ParseArgs(e.Args, out var map, out var ants);
            var main = new StartupWindow(map, ants, isDebug);
            main.Show();
        }

        private static bool ParseArgs(IEnumerable<string> args, out FileSystemInfo map, out IEnumerable<Ant> ants, bool checkSubFolder = true)
        {
            map = null;
            var antList = new List<Ant>();
            var debug = false;

            foreach (var a in args)
            {
                if (a.Equals("debug", StringComparison.InvariantCultureIgnoreCase))
                {
                    debug = true;
                    continue;
                }

                if (!File.Exists(a))
                {
                    if (checkSubFolder && Directory.Exists(a))
                    {
                        ParseArgs(Directory.GetFiles(a), out var subMap, out var subAnts, false);
                        map = subMap;
                        antList.AddRange(subAnts);
                    }
                    continue;
                }

                var info = new FileInfo(a);
                if (info.Extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    var ant = Utilities.LoadAnt(info.FullName);
                    if (ant == null) continue;
                    antList.Add(ant);
                }
                else if (info.Extension.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase))
                {
                    map = info;
                }
            }

            ants = antList.Take(8).ToArray();
            return debug;
        }
    }
}
