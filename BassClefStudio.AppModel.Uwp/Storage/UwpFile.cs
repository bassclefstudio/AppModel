using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Represents an <see cref="IStorageFile"/> wrapper over <see cref="Windows.Storage.IStorageFile"/>.
    /// </summary>
    public class UwpFile : IStorageFile
    {
        private Windows.Storage.IStorageFile File { get; }

        public UwpFile(Windows.Storage.IStorageFile file)
        {
            File = file;
        }

        /// <inheritdoc/>
        public string FileType => File.FileType;

        /// <inheritdoc/>
        public string Name => File.Name;

        /// <inheritdoc/>
        public async Task<IFileContent> OpenFileAsync(FileOpenMode mode = FileOpenMode.Read)
        {
            return new UwpFileContent(await File.OpenAsync(mode.ToUwp()), mode);
        }

        /// <inheritdoc/>
        public async Task<string> ReadTextAsync()
        {
            return await FileIO.ReadTextAsync(File);
        }

        /// <inheritdoc/>
        public async Task WriteTextAsync(string text)
        {
            await FileIO.WriteTextAsync(File, text);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync()
        {
            await File.DeleteAsync();
        }

        public static bool operator ==(UwpFile a, UwpFile b) => a.File == b.File;
        public static bool operator !=(UwpFile a, UwpFile b) => !(a == b);

        public override bool Equals(object obj)
        {
            return obj is UwpFile file
                && this == file;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class UwpFileContent : IFileContent
    {
        private IRandomAccessStream Stream { get; }
        private FileOpenMode OpenMode { get; }

        public UwpFileContent(IRandomAccessStream stream, FileOpenMode openMode)
        {
            Stream = stream;
            OpenMode = openMode;
        }

        /// <inheritdoc/>
        public Stream GetReadStream()
        {
            if (OpenMode == FileOpenMode.Read || OpenMode == FileOpenMode.ReadWrite)
            {
                return Stream.AsStreamForRead();
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
                return Stream.AsStreamForWrite();
            }
            else
            {
                throw new StoragePermissionException($"Creating a writable file stream requires write permission on the file. Permission: {OpenMode}");
            }
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
