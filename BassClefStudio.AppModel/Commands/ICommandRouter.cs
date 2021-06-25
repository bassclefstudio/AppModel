using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// A service responsible for handling the active <see cref="ICommandHandler"/>s in a given app and sending <see cref="CommandRequest"/>s to those registered handlers.
    /// </summary>
    public interface ICommandRouter
    {
        /// <summary>
        /// Gets the collection of currently active <see cref="ICommandHandler"/> instances.
        /// </summary>
        List<ICommandHandler> ActiveCommandHandlers { get; }

        /// <summary>
        /// Initializes this <see cref="ICommandRouter"/> to route <see cref="CommandRequest"/>s from the provided <see cref="IStream{T}"/>.
        /// </summary>
        /// <param name="requestStream">An <see cref="IStream{T}"/> that emits any <see cref="CommandRequest"/>s sent by the app that need to be handled.</param>
        void RouteRequests(IStream<CommandRequest> requestStream);
    }
}
