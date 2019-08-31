using System;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Python
{
    public class Loader : IWrapperLoader
    {
        public string[] Extensions => new[] { "py" };

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Python.PythonAnt",
                ConstructorParameters = new object[] { filename }
            };
        }

        public Func<Ant, AntAction> GetAction()
        {
            return ant =>
            {
                return ((PythonAnt)ant)?.GetAction() ?? AntAction.Wait;
            };
        }
    }
}
