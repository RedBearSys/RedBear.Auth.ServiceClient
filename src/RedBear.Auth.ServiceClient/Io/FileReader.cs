using System;
using System.IO;

namespace RedBear.Auth.ServiceClient.Io
{
    /// <summary>
    /// Implementation of an IFileReader.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    /// <seealso cref="RedBear.Auth.ServiceClient.Io.IFileReader" />
    public class FileReader : IDisposable, IFileReader
    {
        /// <summary>
        /// Gets the underlying TextReader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        public TextReader Reader { get; private set; }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public TextReader Open(string filename)
        {
            Reader = File.OpenText(filename);
            return Reader;
        }

        /// <summary>
        /// Reads the specified cert.
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        public TextReader Read(string cert)
        {
            Reader = new StringReader(cert);
            return Reader;
        }

        public void Dispose()
        {
            Reader?.Dispose();
        }
    }
}
