using BassClefStudio.AppModel.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Represents the system's launcher interface for showing the user links, files, and other content in the default viewer for that platform.
    /// </summary>
    public interface ILauncher
    {
        /// <summary>
        /// Opens the specified <see cref="IStorageFile"/> in whatever app is designed to handle this type of file.
        /// </summary>
        /// <param name="file">The <see cref="IStorageFile"/> file to launch.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        Task<bool> OpenFileAsync(IStorageFile file);

        /// <summary>
        /// Shows the specified <see cref="IStorageFile"/> in the default file explorer, if available.
        /// </summary>
        /// <param name="file">The <see cref="IStorageFile"/> file to launch.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        Task<bool> ShowFileAsync(IStorageFile file);

        /// <summary>
        /// Opens the specified <see cref="IStorageFolder"/> location in the default file explorer, if available.
        /// </summary>
        /// <param name="folder">The <see cref="IStorageFolder"/> folder to launch.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        Task<bool> ShowFolderAsync(IStorageFolder folder);

        /// <summary>
        /// Attempts to open the given <see cref="Uri"/> either in the web browser or (in the case of a deep link) the app registered to handle the given link type.
        /// </summary>
        /// <param name="link">The <see cref="Uri"/> for the system to handle.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        Task<bool> OpenLinkAsync(Uri link);
    }
}
