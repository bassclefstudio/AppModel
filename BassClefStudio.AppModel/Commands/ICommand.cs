using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Represents a implementation of a command based on <see cref="IStream{T}"/> that can be used to manage and trigger a specific action. 
    /// </summary>
    public interface ICommand : IStream<object>
    {
        /// <summary>
        /// An <see cref="IStream{T}"/> that emits <see cref="bool"/> values indicating whether this <see cref="ICommand"/> should be enabled or not.
        /// </summary>
        IStream<bool> EnabledStream { get; }

        /// <summary>
        /// Calls the <see cref="ICommand"/> and executes its associated action.
        /// </summary>
        /// <param name="input">The <see cref="object"/> input passed from the caller to the <see cref="ICommand"/>.</param>
        void Execute(object input = null);

        /// <summary>
        /// A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand"/> provides.
        /// </summary>
        CommandInfo Info { get; }
    }
}
