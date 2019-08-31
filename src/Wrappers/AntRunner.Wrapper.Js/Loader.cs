using System;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Js
{
    [Serializable]
    public class Loader : IWrapperLoader
    {
        public string[] Extensions => new []{"js"};

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Js.JsAnt",
                ConstructorParameters = new object[] { filename }
            };
        }

        public Func<Ant, AntAction> GetAction()
        {
            return ant =>
            {
                return ((JsAnt)ant)?.GetAction() ?? AntAction.Wait;
            };
        }
    }
}
