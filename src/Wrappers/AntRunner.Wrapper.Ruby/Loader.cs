using System;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Wrapper.Ruby
{
    public class Loader : IWrapperLoader
    {
        public string[] Extensions => new[] { "rb" };

        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData
            {
                AssemblyName = Assembly.GetExecutingAssembly().GetName(),
                TypeString = "AntRunner.Wrapper.Ruby.RubyAnt",
                ConstructorParameters = new object[] { filename }
            };
        }

        public Func<Ant, AntAction> GetAction()
        {
            return ant =>
            {
                return ((RubyAnt)ant)?.GetAction() ?? AntAction.Wait;
            };
        }
    }
}
