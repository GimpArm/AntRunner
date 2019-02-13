using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Php
{
    public class Loader : IWrapperLoader
    {
        public string Extension => "php";

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Php.PhpAnt",
                ConstructorParameters = new object[] {filename}
            };
        }
    }
}
