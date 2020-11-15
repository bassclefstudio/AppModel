using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Mvvm.Services.Storage
{
    /// <summary>
    /// A file or directory on the filesystem of a platform.
    /// </summary>
    public interface IStorageItem
    {
        /// <summary>
        /// The name of the <see cref="IStorageItem"/>, including any file extension.
        /// </summary>
        string Name { get; }
    }
}
