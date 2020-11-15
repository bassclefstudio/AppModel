using BassClefStudio.Mvvm.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Mvvm.Services.Storage
{
    //// C# 9 record?
    /// <summary>
    /// An <see cref="IActivatedEventArgs"/> provided when the <see cref="App"/> was started to handle/open a file or folder.
    /// </summary>
    public class StorageActivatedEventArgs : IActivatedEventArgs
    {
        /// <summary>
        /// The <see cref="IStorageItem"/> reference to the activated file or folder.
        /// </summary>
        public IStorageItem AttachedItem { get; }

        /// <summary>
        /// Creates a new <see cref="StorageActivatedEventArgs"/>.
        /// </summary>
        /// <param name="attachedItem">The <see cref="IStorageItem"/> reference to the activated file or folder.</param>
        public StorageActivatedEventArgs(IStorageItem attachedItem)
        {
            AttachedItem = attachedItem;
        }
    }
}
