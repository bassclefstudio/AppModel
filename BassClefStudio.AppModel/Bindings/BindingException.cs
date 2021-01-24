using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// An <see cref="Exception"/> thrown when an error or invalid operation occurs while dealing with <see cref="IBinding{T}"/> data binding.
    /// </summary>
    [Serializable]
    public class BindingException : Exception
    {
        /// <inheritdoc/>
        public BindingException() { }
        /// <inheritdoc/>
        public BindingException(string message) : base(message) { }
        /// <inheritdoc/>
        public BindingException(string message, Exception inner) : base(message, inner) { }
        /// <inheritdoc/>
        protected BindingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
