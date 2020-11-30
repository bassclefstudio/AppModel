using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// A reference to a file on the filesystem of a platform.
    /// </summary>
    public interface IFile : IStorageItem
    {
        /// <summary>
        /// The type of the file, as a file extension (e.g. 'cs').
        /// </summary>
        string FileType { get; }

        /// <summary>
        /// Opens the file attached to this <see cref="IFile"/> and returns the <see cref="IFileContent"/> content.
        /// </summary>
        /// <exception cref="StorageAccessException">An error occurred accessing when attempting to open the file - either it does not exist, or it could not be opened.</exception>
        /// <exception cref="StoragePermissionException">The <see cref="IFile"/> does not have access to this file's <see cref="IFileContent"/>.</exception>
        /// <param name="mode">The <see cref="FileOpenMode"/> describing read and write abilities.</param>
        /// <returns>An <see cref="IFileContent"/> which maps over a stream or native file object and provides methods for reading and writing data.</returns>
        Task<IFileContent> OpenFileAsync(FileOpenMode mode = FileOpenMode.Read);

        /// <summary>
        /// Reads the text from this file asynchronously.
        /// </summary>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>The <see cref="string"/> contents of the file.</returns>
        Task<string> ReadTextAsync();

        /// <summary>
        /// Writes a given <see cref="string"/> to this file asynchronously. Requires <see cref="FileOpenMode.ReadWrite"/> access.
        /// </summary>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <exception cref="StoragePermissionException">The <see cref="IFile"/> does not have write access to the file.</exception>
        /// <param name="text">The <see cref="string"/> text to write to the file.</param>
        Task WriteTextAsync(string text);
    }

    /// <summary>
    /// An enum describing how an <see cref="IFile"/> should be opened.
    /// </summary>
    public enum FileOpenMode
    {
        /// <summary>
        /// The file should be opened in read-only mode.
        /// </summary>
        Read = 0,

        /// <summary>
        /// The file should to be opened in read and write mode.
        /// </summary>
        ReadWrite = 1
    }

    /// <summary>
    /// The content of an opened file, serving as a wrapper around <see cref="System.IO.Stream"/> or another native file object.
    /// </summary>
    public interface IFileContent : IDisposable
    {
        /// <summary>
        /// Gets a reference to a .NET <see cref="Stream"/> that can be used to read from this file.
        /// </summary>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>A <see cref="Stream"/> that can be used to read the file.</returns>
        Stream GetReadStream();

        /// <summary>
        /// Gets a reference to a .NET <see cref="Stream"/> that can be used to write to this file. Requires <see cref="FileOpenMode.ReadWrite"/> access.
        /// </summary>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <exception cref="StoragePermissionException">The <see cref="IFile"/> does not have write access to the file.</exception>
        /// <returns>A <see cref="Stream"/> that can be used to write to the file.</returns>
        Stream GetWriteStream();
    }
}
