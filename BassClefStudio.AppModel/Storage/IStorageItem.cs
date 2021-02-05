﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
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

        /// <summary>
        /// Deletes the <see cref="IStorageItem"/> from the filesystem.
        /// </summary>
        Task RemoveAsync();
    }
}
