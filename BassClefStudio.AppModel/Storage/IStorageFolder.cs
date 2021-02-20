using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// A reference to a directory on the filesystem of a platform.
    /// </summary>
    public interface IStorageFolder : IStorageItem
    {
        /// <summary>
        /// Gets all of the child items of this folder.
        /// </summary>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>An <see cref="IEnumerable{T}"/> of each <see cref="IStorageItem"/> child of the <see cref="IStorageFolder"/>.</returns>
        Task<IEnumerable<IStorageItem>> GetItemsAsync();

        /// <summary>
        /// Gets the child folder in this folder with the given path.
        /// </summary>
        /// <param name="relativePath">The <see cref="string"/> path to the item, relative to this <see cref="IStorageFolder"/>.</param>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>The child <see cref="IStorageFolder"/> at the given path.</returns>
        Task<IStorageFolder> GetFolderAsync(string relativePath);

        /// <summary>
        /// Gets the child file in this folder with the given path.
        /// </summary>
        /// <param name="relativePath">The <see cref="string"/> path to the item, relative to this <see cref="IStorageFolder"/>.</param>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <returns>The child <see cref="IStorageFile"/> at the given path.</returns>
        Task<IStorageFile> GetFileAsync(string relativePath);

        /// <summary>
        /// Creates a new <see cref="IStorageFile"/> in the folder.
        /// </summary>
        /// <param name="name">The name of the new file, including its file extension.</param>
        /// <param name="options">A <see cref="CollisionOptions"/> value indicating the action to take if the file exists.</param>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <exception cref="StorageConflictException">A file with the given <paramref name="name"/> exists, and the <paramref name="options"/> did not provide a way to handle the conflict.</exception>
        /// <returns>The newly created file.</returns>
        Task<IStorageFile> CreateFileAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting);

        /// <summary>
        /// Creates a new <see cref="IStorageFolder"/> in the folder.
        /// </summary>
        /// <param name="name">The name of the new folder.</param>
        /// <param name="options">A <see cref="CollisionOptions"/> value indicating the action to take if the file exists.</param>
        /// <exception cref="StorageAccessException">An error occurred accessing the backend data for the file.</exception>
        /// <exception cref="StorageConflictException">A folder with the given <paramref name="name"/> exists, and the <paramref name="options"/> did not provide a way to handle the conflict.</exception>
        /// <returns>The newly created folder.</returns>
        Task<IStorageFolder> CreateFolderAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting);
    }

    /// <summary>
    /// Represents options on how a <see cref="IStorageItem"/> creation operation can deal with duplicate files.
    /// </summary>
    public enum CollisionOptions
    {
        /// <summary>
        /// Fail if an item exists with the same name.
        /// </summary>
        FailIfExists = 0,
        /// <summary>
        /// Create a new name ("MyFile_1") if an item already exists with the desired name.
        /// </summary>
        RenameIfExists = 1,
        /// <summary>
        /// Overwrite any existing item with the same name.
        /// </summary>
        Overwrite = 2,
        /// <summary>
        /// Open any existing item with the same name.
        /// </summary>
        OpenExisting = 3
    }
}
