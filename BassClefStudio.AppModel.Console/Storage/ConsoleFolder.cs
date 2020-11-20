using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    public class ConsoleFolder : IFolder
    {
        private DirectoryInfo Directory { get; }

        /// <inheritdoc/>
        public string Name => Directory.Name;

        internal ConsoleFolder(DirectoryInfo directory)
        {
            Directory = directory;
            if (!Directory.Exists)
            {
                throw new StorageAccessException("Attempted to create a ConsoleFolder object for a directory that does not exist.");
            }
        }

        /// <inheritdoc/>
        public async Task<IFile> CreateFileAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            var info = new FileInfo(Path.Combine(Directory.FullName, name));
            if (info.Exists && options != CollisionOptions.Overwrite)
            {
                if (options == CollisionOptions.FailIfExists)
                {
                    throw new StorageAccessException($"The requested file {name} already exists.");
                }
                else if (options == CollisionOptions.OpenExisting)
                {
                    return new ConsoleFile(info);
                }
                else if (options == CollisionOptions.RenameIfExists)
                {
                    int num = 1;
                    while (info.Exists)
                    {
                        info = new FileInfo(Path.Combine(Directory.FullName, $"{name}_{num}"));
                        num++;
                    }
                    return new ConsoleFile(info);
                }
                else
                {
                    throw new StorageAccessException($"The given overwrite behavior {options} is not supported by ConsoleFolder.");
                }
            }
            else
            {
                using (var stream = info.CreateText())
                {
                    await stream.WriteAsync(string.Empty);
                    await stream.FlushAsync();
                }
                return new ConsoleFile(info);
            }
        }

        /// <inheritdoc/>
        public async Task<IFolder> CreateFolderAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            var info = new DirectoryInfo(Path.Combine(Directory.FullName, name));
            if (info.Exists && options != CollisionOptions.Overwrite)
            {
                if (options == CollisionOptions.FailIfExists)
                {
                    throw new StorageAccessException($"The requested folder {name} already exists.");
                }
                else if (options == CollisionOptions.OpenExisting)
                {
                    return new ConsoleFolder(info);
                }
                else if (options == CollisionOptions.RenameIfExists)
                {
                    int num = 1;
                    while (info.Exists)
                    {
                        info = new DirectoryInfo(Path.Combine(Directory.FullName, $"{name}_{num}"));
                        num++;
                    }
                    return new ConsoleFolder(info);
                }
                else
                {
                    throw new StorageAccessException($"The given overwrite behavior {options} is not supported by ConsoleFolder.");
                }
            }
            else
            {
                info.Create();
                return new ConsoleFolder(info);
            }
        }

        /// <inheritdoc/>
        public async Task<IStorageItem> GetItemAsync(string relativePath)
        {
            var info = new FileInfo(Path.Combine(Directory.FullName, relativePath));
            if(info.Exists)
            {
                return new ConsoleFile(info);
            }
            else
            {
                throw new StorageAccessException($"The given file {Path.Combine(Directory.FullName, relativePath)} does not exist.");
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IStorageItem>> GetItemsAsync()
        {
            return Directory.EnumerateFiles()
                .Select<FileInfo, IStorageItem>(f => new ConsoleFile(f))
                .Concat(
                Directory.EnumerateDirectories()
                    .Select<DirectoryInfo, IStorageItem>(d => new ConsoleFolder(d)));
        }
    }
}
