namespace AntRunner.Interface
{
    public interface IWrapperLoader
    {
        /// <summary>
        /// The extension this file type can open.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Loads the file and returns an ant object
        /// </summary>
        /// <param name="filename">Full path to the and file</param>
        /// <returns>Ant object for the AppDomain</returns>
        Ant LoadAnt(string filename);
    }
}
