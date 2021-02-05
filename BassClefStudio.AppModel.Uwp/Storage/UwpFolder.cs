﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Represents an <see cref="IStorageFolder"/> wrapper over <see cref="Windows.Storage.IStorageFolder"/>.
    /// </summary>
    public class UwpFolder : IStorageFolder
    {
        public Windows.Storage.IStorageFolder Folder { get; }

        public UwpFolder(Windows.Storage.IStorageFolder folder)
        {
            Folder = folder;
        }

        /// <inheritdoc/>
        public string Name => Folder.Name;

        /// <inheritdoc/>
        public async Task<IStorageFolder> GetFolderAsync(string relativePath)
        {
            try
            {
                return new UwpFolder(await Folder.GetFolderAsync(relativePath));
            }
            catch (Exception ex)
            {
                throw new StorageAccessException($"The given folder {System.IO.Path.Combine(Folder.Path, relativePath)} does not exist.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IStorageFile> GetFileAsync(string relativePath)
        {
            try
            { 
                return new UwpFile(await Folder.GetFileAsync(relativePath));
            }
            catch (Exception ex)
            {
                throw new StorageAccessException($"The given file {System.IO.Path.Combine(Folder.Path, relativePath)} does not exist.", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BassClefStudio.AppModel.Storage.IStorageItem>> GetItemsAsync()
        {
            return (await Folder.GetItemsAsync()).Select(i => i.ToMvvm());
        }

        /// <inheritdoc/>
        public async Task<IStorageFile> CreateFileAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            return new UwpFile(await Folder.CreateFileAsync(name, options.ToUwp()));
        }

        /// <inheritdoc/>
        public async Task<IStorageFolder> CreateFolderAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            return new UwpFolder(await Folder.CreateFolderAsync(name, options.ToUwp()));
        }

        /// <inheritdoc/>
        public async Task<bool> ContainsItemAsync(string relativePath)
        {
            return (await GetItemsAsync()).Any(i => i.Name == relativePath);
        }

        /// <inheritdoc/>
        public async Task RemoveAsync()
        {
            await Folder.DeleteAsync();
        }
    }
}
