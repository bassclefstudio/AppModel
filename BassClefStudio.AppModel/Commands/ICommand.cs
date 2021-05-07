using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Represents a UI command based on the <see cref="IStream{T}"/> interface that can be used to manage and trigger a specific action. 
    /// </summary>
    /// <typeparam name="T">The type of input values this <see cref="ICommand{T}"/> accepts.</typeparam>
    public interface ICommand<T> : IStream<T>
    {
        /// <summary>
        /// An <see cref="IStream{T}"/> that emits <see cref="bool"/> values indicating whether this <see cref="ICommand{T}"/> should be enabled or not.
        /// </summary>
        IStream<bool> EnabledStream { get; }

        /// <summary>
        /// Calls the <see cref="ICommand{T}"/> and executes its associated action.
        /// </summary>
        /// <param name="input">The <typeparamref name="T"/> input passed from the caller to the <see cref="ICommand{T}"/>.</param>
        void InitiateCommand(T input = default(T));

        /// <summary>
        /// A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand{T}"/> provides.
        /// </summary>
        CommandInfo Info { get; }
    }

    /// <summary>
    /// Basic information about a specific <see cref="ICommand{T}"/> the the UI can initiate.
    /// </summary>
    public struct CommandInfo : IIdentifiable<string>
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <summary>
        /// A user-friendly <see cref="string"/> name for the command.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// A short description or tooltip for the action this <see cref="CommandInfo"/> describes.
        /// </summary>
        public string Description { get; set; }
    }
}
