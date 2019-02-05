
using System;
using System.Linq;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class AssemblyLoader : IWrapperLoader
    {
        public string Extension => "dll";
        public Ant LoadAnt(string filename)
        {
            var assembly = Assembly.Load(AssemblyName.GetAssemblyName(filename));

            var antType = assembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Ant)));
            if (antType == null)
            {
                throw new Exception("Could not find class derived from AntRunner.Interface.Ant");
            }

            if (!(Activator.CreateInstance(antType) is Ant ant))
            {
                throw new Exception($"Could not initialize Ant class {antType.Name}");
            }

            return ant;
        }
    }
}
