﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Mvvm.Services.Files
{
    /// <summary>
    /// Represents a reference to a file on the filesystem of a platform.
    /// </summary>
    public interface IFileReference : IStorageItem
    {
        /// <summary>
        /// The type of the file, as a file extension (e.g. 'cs').
        /// </summary>
        string FileType { get; }

        /// <summary>
        /// Opens the file attached to this <see cref="IFileReference"/> and returns the <see cref="IFile"/> content.
        /// </summary>
        /// <exception cref="FileAccessException">An error occurred accessing when attempting to open the file - either it does not exist, or it could not be opened.</exception>
        /// <exception cref="FilePermissionException">The <see cref="IFile"/> does not have access to this file's <see cref="IFile"/>.</exception>
        /// <param name="mode">The <see cref="FileOpenMode"/> describing read and write abilities.</param>
        /// <returns>An <see cref="IFile"/> which maps over a stream or native file object and provides methods for reading and writing data.</returns>
        Task<IFile> OpenFileAsync(FileOpenMode mode = FileOpenMode.Read);
    }

    /// <summary>
    /// An enum describing how an <see cref="IFileReference"/> should be opened.
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
    /// Represents the content of an opened file, serving as a wrapper around <see cref="System.IO.Stream"/> or another native file object.
    /// </summary>
    public interface IFile : IDisposable
    {
        /// <summary>
        /// Reads the text from this file asynchronously.
        /// </summary>
        /// <exception cref="FileAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>The <see cref="string"/> contents of the file.</returns>
        Task<string> ReadTextAsync();

        /// <summary>
        /// Writes a given <see cref="string"/> to this file asynchronously. Requires <see cref="FileOpenMode.ReadWrite"/> access.
        /// </summary>
        /// <exception cref="FileAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <exception cref="FilePermissionException">The <see cref="IFile"/> does not have write access to the file.</exception>
        /// <param name="text">The <see cref="string"/> text to write to the file.</param>
        Task WriteTextAsync(string text);

        /// <summary>
        /// Gets a reference to a .NET <see cref="Stream"/> that can be used to read from this file.
        /// </summary>
        /// <exception cref="FileAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>A <see cref="Stream"/> that can be used to read the file.</returns>
        Stream GetReadStream();

        /// <summary>
        /// Gets a reference to a .NET <see cref="Stream"/> that can be used to write to this file. Requires <see cref="FileOpenMode.ReadWrite"/> access.
        /// </summary>
        /// <exception cref="FileAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <exception cref="FilePermissionException">The <see cref="IFile"/> does not have write access to the file.</exception>
        /// <returns>A <see cref="Stream"/> that can be used to write to the file.</returns>
        Stream GetWriteStream();
    }
}
