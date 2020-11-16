using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Mvvm.Services.Storage
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
        Task<IStorageItem> GetItem(string relativePath);

        /// <summary>
        /// Creates a new <see cref="IFile"/> in the folder.
        /// </summary>
        /// <param name="name">The name of the new file, including its file extension.</param>
        /// <param name="overwrite">A <see cref="bool"/> indicating whether the file contents should be overwritten if the file exists. If 'false', opens the existing <see cref="IFile"/>.</param>
        /// <returns>The newly created file.</returns>
        Task<IFile> CreateFile(string name, bool overwrite = false);

        /// <summary>
        /// Creates a new <see cref="IFolder"/> in the folder.
        /// </summary>
        /// <param name="name">The name of the new folder.</param>
        /// <returns>The newly created folder.</returns>
        Task<IFile> CreateFolder(string name);
    }
}
