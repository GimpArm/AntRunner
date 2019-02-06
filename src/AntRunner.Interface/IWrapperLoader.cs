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
        string Extension { get; }

        /// <summary>
        /// Returns all information needed to load an ant in it's own AppDomain
        /// </summary>
        /// <param name="filename">Path to the file to load.</param>
        /// <returns>AssemblyLoaderData with all information for loading an ant.</returns>
        AssemblyLoaderData MakeLoaderData(string filename);
    }
}
