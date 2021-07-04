using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Basic information about a specific <see cref="ICommand"/> the the UI can initiate.
    /// </summary>
    public class CommandInfo : IIdentifiable<string>
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
            return Id == other.Id;
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
    /// Represents a <see cref="CommandInfo"/> with a strongly-typed input value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandInfo<T> : CommandInfo
    {
        /// <summary>
        /// A user-friendly <see cref="string"/> description of the command's input.
        /// </summary>
        public string InputDescription { get; set; }
    }

    /// <summary>
    /// Represents a single request sent to an <see cref="ICommand{T}"/> from a view or control.
    /// </summary>
    public struct CommandRequest<T>
    {
        /// <summary>
        /// The <see cref="CommandInfo{T}"/> of the command to trigger.
        /// </summary>
        public CommandInfo<T> Command { get; set; }

        /// <summary>
        /// An optional parameter with which to execute the command.
        /// </summary>
        public T Parameter { get; set; }
    }
}
