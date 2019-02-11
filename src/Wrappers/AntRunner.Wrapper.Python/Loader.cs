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
                AssemblyName = typeof(Loader).Assembly.GetName(),
                TypeString = typeof(PythonAnt).FullName,
                ConstructorParameters = new object[] { filename }
        };
        }
    }
}
