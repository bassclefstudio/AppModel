using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when a file, folder, or file content exists, but the current context does not have the permission to access it.
    /// </summary>
    [Serializable]
    public class StoragePermissionException : Exception
    {
        /// <inheritdoc/>
        public StoragePermissionException() { }
        /// <inheritdoc/>
        public StoragePermissionException(string message) : base(message) { }
        /// <inheritdoc/>
        public StoragePermissionException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected StoragePermissionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown when accessing or finding an item in the file system fails.
    /// </summary>
    [Serializable]
    public class StorageAccessException : Exception
    {
        /// <inheritdoc/>
        public StorageAccessException() { }
        /// <inheritdoc/>
        public StorageAccessException(string message) : base(message) { }
        /// <inheritdoc/>
        public StorageAccessException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected StorageAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
