using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Represents a basic <see cref="ICommand"/> which uses <see cref="SourceStream{T}"/> for executing some action when the command is triggered.
    /// </summary>
    public class StreamCommand : ICommand
    {
        /// <summary>
        /// A <see cref="SourceStream{T}"/> which can be used to trigger the action defined by this <see cref="StreamCommand"/>.
        /// </summary>
        public SourceStream<object> TriggerStream { get; }

        /// <inheritdoc/>
        public IStream<bool> EnabledStream { get; }
        
        /// <inheritdoc/>
        public bool Started { get; }

        /// <inheritdoc/>
        public CommandInfo Info { get; }

        /// <inheritdoc/>
        public StreamBinding<object> ValueEmitted { get; }

        /// <summary>
        /// Creates a new <see cref="StreamCommand"/>.
        /// </summary>
        /// <param name="info">A <see cref="CommandInfo"/> object which contains documentation and identifying info for the action this <see cref="ICommand"/> provides.</param>
        /// <param name="enableStream">An <see cref="IStream{T}"/> that emits <see cref="bool"/> values indicating whether this <see cref="StreamCommand"/> should be enabled or not.</param>
        public StreamCommand(CommandInfo info, IStream<bool> enableStream = null)
        {
            Info = info;
            TriggerStream = new SourceStream<object>();
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
        public void Execute(object input = null)
        {
            TriggerStream.EmitValue(input);
        }
    }
}
