using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Mvvm.Services.Files
{
    /// <summary>
    /// Represents a reference to a directory on the filesystem of a platform.
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
    }
}
