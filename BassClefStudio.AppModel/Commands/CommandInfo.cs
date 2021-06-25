using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
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

    /// <summary>
    /// Represents a single request sent to an <see cref="ICommand"/> from a view or control.
    /// </summary>
    public struct CommandRequest
    {
        /// <summary>
        /// The <see cref="CommandInfo"/> of the command to trigger.
        /// </summary>
        public CommandInfo Command { get; set; }

        /// <summary>
        /// An optional parameter with which to execute the command.
        /// </summary>
        public object Parameter { get; set; }
    }
}
