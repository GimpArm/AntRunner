using System;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Js
{
    [Serializable]
    public class Loader : IWrapperLoader
    {
        public string Extension => "js";

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = typeof(Loader).Assembly.GetName(),
                TypeString = typeof(JsAnt).FullName,
                ConstructorParameters = new object[] { filename }
            };
        }
    }
}
