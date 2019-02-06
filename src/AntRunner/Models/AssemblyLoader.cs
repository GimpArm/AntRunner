using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class AssemblyLoader : IWrapperLoader
    {
        public string Extension => "dll";

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData {AssemblyName = AssemblyName.GetAssemblyName(filename) };
        }
    }
}
