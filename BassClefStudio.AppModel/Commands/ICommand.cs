using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Represents a UI command based on <see cref="IStream{T}"/> that can be used to manage and trigger a specific action. 
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
        void InitiateCommand(object input = null);

        /// <summary>
        /// A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand"/> provides.
        /// </summary>
        CommandInfo Info { get; }
    }

    /// <summary>
    /// Basic information about a specific <see cref="ICommand"/> the the UI can initiate.
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
        public override int GetHashCode()
        {
            int hashCode = -1805587026;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FriendlyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
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
