using BassClefStudio.AppModel.Commands;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// Provides command routing and a static <see cref="SourceStream{T}"/> which is used by default by the <see cref="ICommandRouter"/> when routing <see cref="CommandRequest"/>s.
    /// </summary>
    public class CommandRouter : ICommandRouter
    {
        /// <summary>
        /// A <see cref="SourceStream{T}"/> that emits <see cref="CommandRequest"/> command requests from the app.
        /// </summary>
        public static SourceStream<CommandRequest> RequestStream { get; } = new SourceStream<CommandRequest>();
        
        /// <inheritdoc/>
        public List<ICommandHandler> ActiveCommandHandlers { get; }

        /// <summary>
        /// Creates a new <see cref="CommandRouter"/> instance.
        /// </summary>
        public CommandRouter()
        {
            ActiveCommandHandlers = new List<ICommandHandler>();
            RouteRequests(RequestStream);
        }

        /// <inheritdoc/>
        public void RouteRequests(IStream<CommandRequest> requestStream)
        {
            requestStream.BindResult(r => ActiveCommandHandlers.ExecuteCommand(r.Command, r.Parameter));
        }
    }
}
