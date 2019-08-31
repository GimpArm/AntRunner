using System;
using System.Reflection;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class AssemblyLoader : IWrapperLoader
    {
        public string[] Extensions => new [] { "dll" };


        public AssemblyLoaderData MakeLoaderData(string filename)
        {
            return new AssemblyLoaderData {AssemblyName = AssemblyName.GetAssemblyName(filename) };
        }

        public Func<Ant, AntAction> GetAction()
        {
            return ant => ant.Action;
        }
    }
}
