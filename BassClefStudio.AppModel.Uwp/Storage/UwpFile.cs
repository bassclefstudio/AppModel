using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Represents an <see cref="IFile"/> wrapper over <see cref="IStorageFile"/>.
    /// </summary>
    public class UwpFile : IFile
    {
        private IStorageFile File { get; }

        public UwpFile(IStorageFile file)
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
            return new UwpFileContent(File, mode);
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
        private IStorageFile File { get; }
        private FileOpenMode OpenMode { get; }

        public UwpFileContent(IStorageFile file, FileOpenMode openMode)
        {
            File = file;
            OpenMode = openMode;
        }

        /// <inheritdoc/>
        public async Task<Stream> GetReadStream()
        {
            if (OpenMode == FileOpenMode.Read || OpenMode == FileOpenMode.ReadWrite)
            {
                var uwpStream = await File.OpenAsync(FileAccessMode.ReadWrite);
                return uwpStream.AsStreamForRead();
            }
            else
            {
                throw new StoragePermissionException($"Creating a readable file stream requires read permission on the file. Permission: {OpenMode}");
            }
        }

        /// <inheritdoc/>
        public async Task<Stream> GetWriteStream()
        {
            if (OpenMode == FileOpenMode.ReadWrite)
            {
                var uwpStream = await File.OpenAsync(FileAccessMode.ReadWrite);
                return uwpStream.AsStreamForWrite();
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
                return await FileIO.ReadTextAsync(File);
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
                await FileIO.WriteTextAsync(File, text);
            }
            else
            {
                throw new StoragePermissionException($"Creating a writable file stream requires write permission on the file. Permission: {OpenMode}");
            }
        }

        public void Dispose()
        {
            //Stream.Dispose();
        }
    }
}
