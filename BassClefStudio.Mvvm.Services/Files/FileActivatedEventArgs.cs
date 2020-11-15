using BassClefStudio.Mvvm.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Mvvm.Services.Files
{
    //// C# 9 record?
    /// <summary>
    /// An <see cref="IActivatedEventArgs"/> provided when the <see cref="App"/> was started to handle/open a file or folder.
    /// </summary>
    public class FileActivatedEventArgs : IActivatedEventArgs
    {
        /// <summary>
        /// The <see cref="IStorageItem"/> reference to the activated file or folder.
        /// </summary>
        public IStorageItem AttachedItem { get; }

        /// <summary>
        /// Creates a new <see cref="FileActivatedEventArgs"/>.
        /// </summary>
        /// <param name="attachedItem">The <see cref="IStorageItem"/> reference to the activated file or folder.</param>
        public FileActivatedEventArgs(IStorageItem attachedItem)
        {
            AttachedItem = attachedItem;
        }
    }
}
