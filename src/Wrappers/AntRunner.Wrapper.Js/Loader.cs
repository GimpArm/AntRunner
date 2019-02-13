using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Js
{
    public class Loader : IWrapperLoader
    {
        public string Extension => "js";

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Js.JsAnt",
                ConstructorParameters = new object[] { filename }
            };
        }
    }
}
