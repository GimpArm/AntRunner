using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Python
{
    public class Loader : IWrapperLoader
    {
        public string Extension => "py";

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Python.PythonAnt",
                ConstructorParameters = new object[] { filename }
        };
        }
    }
}
