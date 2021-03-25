using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Represents a service that can provide an external (i.e. not the default file-system) implementation of <see cref="IStorageItem"/> objects and navigate a collection of files and folders.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="IStorageProvider"/> has been initialized (see <see cref="InitializeAsync"/>).
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Attempts to asynchronously initialize the external filesystem this <see cref="IStorageProvider"/> provides.
        /// </summary>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        Task<bool> InitializeAsync();

        /// <summary>
        /// Asynchronously returns the root node of the filesystem, as an <see cref="IStorageFolder"/>.
        /// </summary>
        /// <returns>An <see cref="IStorageFolder"/> which may or may not be an actual folder, but which contains the contents of the <see cref="IStorageProvider"/>'s filesystem.</returns>
        Task<IStorageFolder> GetRootAsync();
    }
}
