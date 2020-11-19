using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace BassClefStudio.AppModel.Storage.Uwp
{
    /// <summary>
    /// Represents an <see cref="IFolder"/> wrapper over <see cref="IStorageFolder"/>.
    /// </summary>
    public class UwpFolder : IFolder
    {
        public IStorageFolder Folder { get; }

        public UwpFolder(IStorageFolder folder)
        {
            Folder = folder;
        }

        /// <inheritdoc/>
        public string Name => Folder.Name;

        /// <inheritdoc/>
        public async Task<BassClefStudio.AppModel.Storage.IStorageItem> GetItemAsync(string relativePath)
        {
            return (await Folder.GetItemAsync(relativePath)).ToMvvm();
            
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<BassClefStudio.AppModel.Storage.IStorageItem>> GetItemsAsync()
        {
            return (await Folder.GetItemsAsync()).Select(i => i.ToMvvm());
        }

        /// <inheritdoc/>
        public async Task<IFile> CreateFileAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            return new UwpFile(await Folder.CreateFileAsync(name, options.ToUwp()));
        }

        /// <inheritdoc/>
        public async Task<IFolder> CreateFolderAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting)
        {
            return new UwpFolder(await Folder.CreateFolderAsync(name, options.ToUwp()));
        }
    }
}
