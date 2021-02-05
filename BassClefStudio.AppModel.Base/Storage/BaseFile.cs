using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// An <see cref="IStorageFile"/> implementation that uses the .NET <see cref="FileInfo"/> class for creating and managing a file.
    /// </summary>
    public class BaseFile : IStorageFile
    {
        private FileInfo File { get; }

        /// <inheritdoc/>
        public string Name => File.Name;

        /// <inheritdoc/>
        public string FileType => File.Extension;

        /// <summary>
        /// Creates a <see cref="BaseFile"/> from the given file.
        /// </summary>
        /// <param name="file">The .NET <see cref="FileInfo"/> file.</param>
        public BaseFile(FileInfo file)
        {
            File = file;
            if (!File.Exists)
            {
                File.Create().Dispose();
            }
        }

        /// <inheritdoc/>
        public async Task<IFileContent> OpenFileAsync(FileOpenMode mode = FileOpenMode.Read)
        {
            if(mode == FileOpenMode.ReadWrite)
            {
                return new BaseFileContent(File, mode);
            }
            else
            {
                return new BaseFileContent(File, mode);
            }
        }

        /// <inheritdoc/>
        public async Task<string> ReadTextAsync()
        {
            return System.IO.File.ReadAllText(File.FullName);
        }

        /// <inheritdoc/>
        public async Task WriteTextAsync(string text)
        {
            System.IO.File.WriteAllText(File.FullName, text);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync()
        {
            File.Delete();
        }

        public static bool operator ==(BaseFile a, BaseFile b) => a.File == b.File;
        public static bool operator !=(BaseFile a, BaseFile b) => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is BaseFile file
                && this == file;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a basic <see cref="IFileContent"/> wrapper over the .NET <see cref="Stream"/> class.
    /// </summary>
    public class BaseFileContent : IFileContent
    {
        /// <summary>
        /// The attached .NET <see cref="FileInfo"/> for the file.
        /// </summary>
        public FileInfo File { get; }

        private FileOpenMode OpenMode { get; }

        /// <summary>
        /// Creates a new <see cref="BaseFileContent"/>
        /// </summary>
        /// <param name="file">The attached .NET <see cref="FileInfo"/> for the file.</param>
        /// <param name="openMode">The mode with which this file/stream was opened.</param>
        public BaseFileContent(FileInfo file, FileOpenMode openMode)
        {
            OpenMode = openMode;
            File = file;
        }

        /// <inheritdoc/>
        public Stream GetReadStream()
        {
            if (OpenMode == FileOpenMode.Read || OpenMode == FileOpenMode.ReadWrite)
            {
                return File.OpenRead();
            }
            else
            {
                throw new StoragePermissionException($"Creating a readable file stream requires read permission on the file. Permission: {OpenMode}");
            }
        }

        /// <inheritdoc/>
        public Stream GetWriteStream()
        {
            if (OpenMode == FileOpenMode.ReadWrite)
            {
                return File.OpenWrite();
            }
            else
            {
                throw new StoragePermissionException($"Creating a writable file stream requires write permission on the file. Permission: {OpenMode}");
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
