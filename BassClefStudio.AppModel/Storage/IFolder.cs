using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// A reference to a directory on the filesystem of a platform.
    /// </summary>
    public interface IFolder : IStorageItem
    {
        /// <summary>
        /// Gets all of the child items of this folder.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of each <see cref="IStorageItem"/> child of the <see cref="IFolder"/>.</returns>
        Task<IEnumerable<IStorageItem>> GetItemsAsync();

        /// <summary>
        /// Gets the child item in this folder with the given path.
        /// </summary>
        /// <param name="relativePath">The <see cref="string"/> path to the item, relative to this <see cref="IFolder"/>.</param>
        /// <returns>the child <see cref="IStorageItem"/> at the given path.</returns>
        Task<IStorageItem> GetItemAsync(string relativePath);

        /// <summary>
        /// Creates a new <see cref="IFile"/> in the folder.
        /// </summary>
        /// <param name="name">The name of the new file, including its file extension.</param>
        /// <param name="options">A <see cref="CollisionOptions"/> value indicating the action to take if the file exists.</param>
        /// <returns>The newly created file.</returns>
        Task<IFile> CreateFileAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting);

        /// <summary>
        /// Creates a new <see cref="IFolder"/> in the folder.
        /// </summary>
        /// <param name="name">The name of the new folder.</param>
        /// <param name="options">A <see cref="CollisionOptions"/> value indicating the action to take if the file exists.</param>
        /// <returns>The newly created folder.</returns>
        Task<IFolder> CreateFolderAsync(string name, CollisionOptions options = CollisionOptions.OpenExisting);
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
