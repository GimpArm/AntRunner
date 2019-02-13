using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Ruby
{
    public class Loader : IWrapperLoader
    {
        public string Extension => "rb";

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Ruby.RubyAnt",
                ConstructorParameters = new object[] { filename }
        };
        }
    }
}
