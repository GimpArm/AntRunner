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

        private static bool ParseArgs(IEnumerable<string> args, out FileSystemInfo map, out IDictionary<AntProxy, AppDomain> ants, bool checkSubFolder = true)
        {
            map = null;
            var antList = new Dictionary<AntProxy, AppDomain>();
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
                        foreach (var v in subAnts)
                        {
                            antList.Add(v.Key, v.Value);
                        }
                    }
                    continue;
                }

                var info = new FileInfo(a);
                if (info.Extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    var ant = AntLoader.Load(info.FullName, out var domain);
                    if (ant == null) continue;
                    antList.Add(ant, domain);
                }
                else if (info.Extension.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase))
                {
                    map = info;
                }
            }

            ants = antList.Take(8).ToDictionary(x => x.Key, y => y.Value);
            return debug;
        }
    }
}
