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
                AssemblyName = typeof(Loader).Assembly.GetName(),
                TypeString = typeof(PhpAnt).FullName,
                ConstructorParameters = new object[] {filename}
            };
        }
    }
}
