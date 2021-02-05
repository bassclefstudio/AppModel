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
    }
}
