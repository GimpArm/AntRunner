using System;

namespace AntRunner.Interface
{
    /// <summary>
    /// Wrapper to make custom loaders for different programming languages.
    /// </summary>
    public interface IWrapperLoader
    {
        /// <summary>
        /// The extension this file type can open.
        /// </summary>
        string[] Extensions { get; }

        /// <summary>
        /// Returns all information needed to load an ant in it's own AppDomain
        /// </summary>
        /// <param name="filename">Path to the file to load.</param>
        /// <returns>AssemblyLoaderData with all information for loading an ant.</returns>
        AssemblyLoaderData MakeLoaderData(string filename);

        /// <summary>
        /// Returns a function which can be internally called to get the AntAction. This is to make it more fair for the wrapped ants by allowing async Action setting.
        /// </summary>
        /// <returns>Function that when called returns the next action.</returns>
        Func<Ant, AntAction> GetAction();
    }
}
