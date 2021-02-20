using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Extension methods for the <see cref="IStorageItem"/> and related interfaces.
    /// </summary>
    public static class StorageExtensions
    {
        /// <summary>
        /// Gets the <see cref="IStorageItem.Name"/> of the file without the extension.
        /// </summary>
        /// <param name="file">The <see cref="IStorageFile"/> to find the name of.</param>
        public static string GetNameWithoutExtension(this IStorageFile file)
        {
            return Path.GetFileNameWithoutExtension(file.Name);
        }

        /// <summary>
        /// Checks to see if the file or folder at the given path is in the <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="folder">The <see cref="IStorageFolder"/> folder to query.</param>
        /// <param name="name">The <see cref="string"/> name of the item in this <see cref="IStorageFolder"/>.</param>
        public static async Task<bool> ContainsItemAsync(this IStorageFolder folder, string name)
        {
            var allItems = await folder.GetItemsAsync();
            return allItems.Any(i => i.Name == name);
        }

        #region Copy and Move

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into a new file located in the specified <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="file">The existing file whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="fileName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="file"/>).</param>
        public static async Task CopyToAsync(this IStorageFile file, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string fileName = null)
        {
            IStorageFile destinationFile = await destination.CreateFileAsync(fileName ?? file.Name, collisionOptions);
            await CopyContentsAsync(file, destinationFile);
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into a new file located in the specified <see cref="IStorageFolder"/>, and then removes the source file.
        /// </summary>
        /// <param name="file">The existing file whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="fileName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="file"/>).</param>
        public static async Task MoveToAsync(this IStorageFile file, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string fileName = null)
        {
            IStorageFile destinationFile = await destination.CreateFileAsync(fileName ?? file.Name, collisionOptions);
            await CopyContentsAsync(file, destinationFile);
            await file.RemoveAsync();
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into the provided new <see cref="IStorageFile"/>.
        /// </summary>
        /// <param name="file">The existing file whose contents should be read.</param>
        /// <param name="destination">A new, empty file with write access, where the contents of <paramref name="file"/> will be copied.</param>
        public static async Task CopyContentsAsync(this IStorageFile file, IStorageFile destination)
        {
            using (var readFile = await file.OpenFileAsync(FileOpenMode.Read))
            {
                using (var readStream = readFile.GetReadStream())
                {
                    using (var writeFile = await destination.OpenFileAsync(FileOpenMode.ReadWrite))
                    {
                        using (var writeStream = writeFile.GetWriteStream())
                        {
                            await readStream.CopyToAsync(writeStream);
                            await writeStream.FlushAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFile"/> into a new folder located in the specified <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="folder">The existing file whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="folderName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="folder"/>).</param>
        public static async Task CopyToAsync(this IStorageFolder folder, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string folderName = null)
        {
            IStorageFolder destinationFolder = await destination.CreateFolderAsync(folderName ?? folder.Name, collisionOptions);
            await CopyContentsAsync(folder, destinationFolder);
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFolder"/> into a new folder located in the specified <see cref="IStorageFolder"/>, and then removes the source folder.
        /// </summary>
        /// <param name="folder">The existing folder whose contents should be read.</param>
        /// <param name="destination">The destination folder to copy the <see cref="IStorageFile"/> into.</param>
        /// <param name="collisionOptions">Overrides the <see cref="CollisionOptions"/> behavior when creating the new file in the <paramref name="destination"/> folder.</param>
        /// <param name="folderName">Overrides the name of the destination file (default is <see cref="IStorageItem.Name"/> of the source <paramref name="folder"/>).</param>
        public static async Task MoveToAsync(this IStorageFolder folder, IStorageFolder destination, CollisionOptions collisionOptions = CollisionOptions.RenameIfExists, string folderName = null)
        {
            IStorageFolder destinationFolder = await destination.CreateFolderAsync(folderName ?? folder.Name, collisionOptions);
            await CopyContentsAsync(folder, destinationFolder);
            await folder.RemoveAsync();
        }

        /// <summary>
        /// Copies the contents of the given <see cref="IStorageFolder"/> into the provided new <see cref="IStorageFolder"/>.
        /// </summary>
        /// <param name="folder">The existing folder whose contents should be read.</param>
        /// <param name="destination">A new, empty folder with write access, where the contents of <paramref name="folder"/> will be copied.</param>
        public static async Task CopyContentsAsync(this IStorageFolder folder, IStorageFolder destination)
        {
            foreach(var item in await folder.GetItemsAsync())
            {
                if(item is IStorageFile file)
                {
                    await file.CopyToAsync(destination, CollisionOptions.FailIfExists);
                }
                else if(item is IStorageFolder dir)
                {
                    await dir.CopyToAsync(destination, CollisionOptions.FailIfExists);
                }
                else
                {
                    throw new StorageAccessException($"Attempted to copy an item that was neither a file or folder. Type \"{item?.GetType().Name}\"");
                }
            }
        }

        #endregion
    }
}
