using System.IO;

namespace RedBear.Auth.ServiceClient.Io
{
    /// <summary>
    /// Interface for file IO to allow easier unit testing and mocking.
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// Gets the underlying TextReader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        TextReader Reader { get; }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        TextReader Open(string filename);
        
        /// <summary>
        /// Reads the specified cert.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        TextReader Read(string cert);
    }
}
