using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    public class ConsoleFile : IFile
    {
        private FileInfo File { get; }

        /// <inheritdoc/>
        public string Name => File.Name;

        /// <inheritdoc/>
        public string FileType => File.Extension;

        internal ConsoleFile(FileInfo file)
        {
            File = file;
            if(!File.Exists)
            {
                throw new StorageAccessException("Attempted to create a ConsoleFile object for a file that does not exist.");
            }
        }

        /// <inheritdoc/>
        public async Task<IFileContent> OpenFileAsync(FileOpenMode mode = FileOpenMode.Read)
        {
            if(mode == FileOpenMode.ReadWrite)
            {
                return new ConsoleFileContent(File, mode);
            }
            else
            {
                return new ConsoleFileContent(File, mode);
            }
        }
    }

    /// <summary>
    /// Represents a basic <see cref="IFileContent"/> wrapper over the .NET <see cref="Stream"/> class.
    /// </summary>
    public class ConsoleFileContent : IFileContent
    {
        /// <summary>
        /// The attached .NET <see cref="FileInfo"/> for the file.
        /// </summary>
        public FileInfo File { get; }

        private FileOpenMode OpenMode { get; }

        /// <summary>
        /// Creates a new <see cref="ConsoleFileContent"/>
        /// </summary>
        /// <param name="file">The attached .NET <see cref="FileInfo"/> for the file.</param>
        /// <param name="openMode">The mode with which this file/stream was opened.</param>
        public ConsoleFileContent(FileInfo file, FileOpenMode openMode)
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
        public async Task<string> ReadTextAsync()
        {
            if (OpenMode == FileOpenMode.Read || OpenMode == FileOpenMode.ReadWrite)
            {
                using (var stream = GetReadStream())
                {
                    using (var textReader = new StreamReader(stream))
                    {
                        return await textReader.ReadToEndAsync();
                    }
                }
            }
            else
            {
                throw new StoragePermissionException($"Creating a readable file stream requires read permission on the file. Permission: {OpenMode}");
            }
        }

        /// <inheritdoc/>
        public async Task WriteTextAsync(string text)
        {
            if (OpenMode == FileOpenMode.ReadWrite)
            {
                using (var stream = GetWriteStream())
                {
                    using (var textWriter = new StreamWriter(stream))
                    {
                        await textWriter.WriteAsync(text);
                        await textWriter.FlushAsync();
                    }
                }
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
