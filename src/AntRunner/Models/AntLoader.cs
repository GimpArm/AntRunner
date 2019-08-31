using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public static class AntLoader
    {
        private static IList<IWrapperLoader> _wrappers;
        public static IList<IWrapperLoader> Wrappers => _wrappers ?? (_wrappers = GetWrappers());

        private static string _dialogFilter;
        public static string DialogFilter => _dialogFilter ?? (_dialogFilter = string.Format("Ant files ({0})|{0}|All files (*.*)|*.*", string.Join(";", Wrappers.SelectMany(x => x.Extensions).Select(x => $"*.{x.ToLower()}"))));

        public static IWrapperLoader GetLoader(string filename)
        {
            var info = new FileInfo(filename);
            var extension = info.Extension.TrimStart('.');
            return Wrappers.FirstOrDefault(x => x.Extensions.Any(y => y.Equals(extension, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static bool ValidExtension(string extension)
        {
            extension = extension.TrimStart('.');
            return Wrappers.Any(x => x.Extensions.Any(y => y.Equals(extension, StringComparison.InvariantCultureIgnoreCase)));
        }

        public static AntProxy Load(string filename, out AppDomain domain)
        {
            try
            {
                var d = AppDomain.CreateDomain($"LoadingDomain-{DateTime.Now.Ticks}", AppDomain.CurrentDomain.Evidence, new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory });
                d.Load(AssemblyName.GetAssemblyName(typeof(Ant).Assembly.Location));
                d.Load(AssemblyName.GetAssemblyName(typeof(Newtonsoft.Json.JsonConverter).Assembly.Location));
                var loader = GetLoader(filename);
                if (loader == null) throw new Exception("Invalid file type");

                var data = loader.MakeLoaderData(filename);
                // ReSharper disable once AssignNullToNotNullAttribute
                var antProxy = (AntProxy)d.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AntProxy).FullName);
                antProxy.LoadAssembly(data);

                domain = d;
                antProxy.SetGetAction(loader.GetAction());
                return antProxy;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                domain = null;
                return null;
            }
        }

        private static IList<IWrapperLoader> GetWrappers()
        {
            var result = new List<IWrapperLoader> {new AssemblyLoader()};
            var loaders = ConfigurationManager.GetSection("wrapperLoader") as WrapperLoaderSection;
            var baseFolder = new FileInfo(typeof(App).Assembly.Location);
            if (loaders == null || baseFolder.DirectoryName == null) return result;

            foreach (var wrapper in loaders.Wrappers.OfType<Wrapper>())
            {
                try
                {
                    var path = Path.Combine(baseFolder.DirectoryName, "Wrappers", wrapper.Path);
                    var assembly = Assembly.Load(AssemblyName.GetAssemblyName(path));
                    var loader = Activator.CreateInstance(assembly.GetType(wrapper.Type)) as IWrapperLoader;
                    if (loader == null) continue;
                    result.Add(loader);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Invalid Wrapper {wrapper.Type}\r\n{e.Message}", "Invalid Wrapper", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return result;
        }
    }
}
