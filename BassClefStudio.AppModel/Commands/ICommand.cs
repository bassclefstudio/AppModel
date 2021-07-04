using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// The base interface all <see cref="ICommand{T}"/>s, regardless of input type, implement.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// An <see cref="IStream{T}"/> that emits <see cref="bool"/> values indicating whether this <see cref="ICommand"/> should be enabled or not.
        /// </summary>
        IStream<bool> EnabledStream { get; }

        /// <summary>
        /// A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand"/> provides.
        /// </summary>
        CommandInfo Info { get; }

        /// <summary>
        /// Calls the <see cref="ICommand"/> and executes its associated action.
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// Represents an implementation of a command based on <see cref="IStream{T}"/> that can be used to manage and trigger a specific action. 
    /// </summary>
    /// <typeparam name="T">The type of inputs this <see cref="ICommand{T}"/> accepts.</typeparam>
    public interface ICommand<T> : ICommand, IStream<T>
    {
        /// <summary>
        /// Calls the <see cref="ICommand{T}"/> and executes its associated action.
        /// </summary>
        /// <param name="input">The <typeparamref name="T"/> input passed from the caller to the <see cref="ICommand{T}"/>.</param>
        void Execute(T input = default(T));

        /// <summary>
        /// A <see cref="CommandInfo{T}"/> object which contains documentation and identifying info for the action this <see cref="ICommand"/> provides.
        /// </summary>
        CommandInfo<T> Info { get; }
    }
}
