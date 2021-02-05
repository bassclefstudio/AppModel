using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}
