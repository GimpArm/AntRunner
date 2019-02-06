using System;
using System.Reflection;

namespace AntRunner.Interface
{
    /// <summary>
    /// Class containing data needed to load an ant.
    /// </summary>
    [Serializable]
    public class AssemblyLoaderData
    {
        /// <summary>
        /// String of the full name of the ant class.
        /// </summary>
        public string TypeString { get; set; }

        /// <summary>
        /// AssemblyName information of the assembly containing the ant class.
        /// </summary>
        public AssemblyName AssemblyName { get; set; }

        /// <summary>
        /// Object array of the parameters needed for the ant constructor.
        /// </summary>
        public object[] ConstructorParameters { get; set; }
    }
}
