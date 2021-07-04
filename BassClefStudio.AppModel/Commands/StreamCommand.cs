using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Represents a basic <see cref="ICommand{T}"/> which uses <see cref="SourceStream{T}"/> for executing some action when the command is triggered.
    /// </summary>
    /// <typeparam name="T">The type of inputs this <see cref="ICommand{T}"/> accepts.</typeparam>
    public class StreamCommand<T> : ICommand<T>
    {
        /// <summary>
        /// A <see cref="SourceStream{T}"/> which can be used to trigger the action defined by this <see cref="StreamCommand{T}"/>.
        /// </summary>
        public SourceStream<T> TriggerStream { get; }

        /// <inheritdoc/>
        public IStream<bool> EnabledStream { get; }
        
        /// <inheritdoc/>
        public bool Started { get; }

        /// <inheritdoc/>
        CommandInfo ICommand.Info => Info;
        /// <inheritdoc/>
        public CommandInfo<T> Info { get; }

        /// <inheritdoc/>
        public StreamBinding<T> ValueEmitted { get; }

        /// <summary>
        /// Creates a new <see cref="StreamCommand{T}"/>.
        /// </summary>
        /// <param name="info">A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand{T}"/> provides.</param>
        /// <param name="enableStream">An <see cref="IStream{T}"/> that emits <see cref="bool"/> values indicating whether this <see cref="StreamCommand{T}"/> should be enabled or not.</param>
        public StreamCommand(CommandInfo<T> info, IStream<bool> enableStream = null)
        {
            Info = info;
            TriggerStream = new SourceStream<T>();
            ValueEmitted = TriggerStream.ValueEmitted;
            EnabledStream = enableStream ?? true.AsStream();
        }

        /// <inheritdoc/>
        public void Start()
        {
            TriggerStream.Start();
            EnabledStream.Start();
        }

        /// <inheritdoc/>
        public void Execute(T input = default(T))
        {
            TriggerStream.EmitValue(input);
        }
    }
}
