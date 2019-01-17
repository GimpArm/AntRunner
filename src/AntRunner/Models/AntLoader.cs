using System;
using System.Reflection;
using System.Windows;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public static class AntLoader
    {
        public static AntProxy Load(string filename, out AppDomain domain)
        {
            try
            {
                var d = AppDomain.CreateDomain($"LoadingDomain-{DateTime.Now.Ticks}", AppDomain.CurrentDomain.Evidence, new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory });
                d.Load(AssemblyName.GetAssemblyName(typeof(Ant).Assembly.Location));
                
                // ReSharper disable once AssignNullToNotNullAttribute
                var antProxy = (AntProxy)d.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AntProxy).FullName);
                antProxy.LoadAssembly(filename);

                domain = d;
                return antProxy;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                domain = null;
                return null;
            }
        }
    }
}
