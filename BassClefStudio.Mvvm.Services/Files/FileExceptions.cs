using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Mvvm.Services.Files
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when a file, folder, or file content exists, but the current context does not have the permission to access it.
    /// </summary>
    [Serializable]
    public class FilePermissionException : Exception
    {
        /// <inheritdoc/>
        public FilePermissionException() { }
        /// <inheritdoc/>
        public FilePermissionException(string message) : base(message) { }
        /// <inheritdoc/>
        public FilePermissionException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected FilePermissionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown when accessing or finding an item in the file system fails.
    /// </summary>
    [Serializable]
    public class FileAccessException : Exception
    {
        /// <inheritdoc/>
        public FileAccessException() { }
        /// <inheritdoc/>
        public FileAccessException(string message) : base(message) { }
        /// <inheritdoc/>
        public FileAccessException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected FileAccessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
