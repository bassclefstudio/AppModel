using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Represents a UI command that can be used to manage and trigger a specific action. 
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// An <see cref="IStream{T}"/> that emits <see cref="bool"/> values indicating whether this <see cref="ICommand"/> should be enabled or not.
        /// </summary>
        IStream<bool> EnabledStream { get; }

        /// <summary>
        /// Calls the <see cref="ICommand"/> and executes its associated action.
        /// </summary>
        /// <param name="input">The <see cref="object"/> input passed from the caller to the <see cref="ICommand"/>.</param>
        void InitiateCommand(object input = null);

        /// <summary>
        /// A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand{T}"/> provides.
        /// </summary>
        CommandInfo Info { get; }
    }

    /// <summary>
    /// Represents a UI command based on the <see cref="IStream{T}"/> interface that can be used to manage and trigger a specific action. 
    /// </summary>
    /// <typeparam name="T">The strongly-typed input values this <see cref="ICommand{T}"/> accepts.</typeparam>
    public interface ICommand<T> : ICommand, IStream<T>
    {
        /// <summary>
        /// Calls the <see cref="ICommand{T}"/> and executes its associated action.
        /// </summary>
        /// <param name="input">The <typeparamref name="T"/> input passed from the caller to the <see cref="ICommand{T}"/>.</param>
        void InitiateCommand(T input = default(T));
    }

    /// <summary>
    /// Basic information about a specific <see cref="ICommand{T}"/> the the UI can initiate.
    /// </summary>
    public struct CommandInfo : IIdentifiable<string>, IEquatable<CommandInfo>
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

        #region Overrides

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Id} ({FriendlyName})";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is CommandInfo info && Equals(info);
        }

        /// <inheritdoc/>
        public bool Equals(CommandInfo other)
        {
            return Id == other.Id &&
                   FriendlyName == other.FriendlyName &&
                   Description == other.Description;
        }

        /// <inheritdoc/>
        public static bool operator ==(CommandInfo left, CommandInfo right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(CommandInfo left, CommandInfo right)
        {
            return !(left == right);
        }

        #endregion
    }
}
