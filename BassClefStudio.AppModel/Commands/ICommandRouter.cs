using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        IList<ICommandHandler> ActiveCommandHandlers { get; }

        /// <summary>
        /// Retrieves the current <see cref="ICommand"/> instance routed to the given <see cref="CommandInfo"/> command.
        /// </summary>
        /// <param name="command">The <see cref="CommandInfo"/> command definition.</param>
        /// <returns>An <see cref="ICommand"/> (usually from <see cref="ActiveCommandHandlers"/>) instance of the desired command.</returns>
        ICommand GetCommand(CommandInfo command);
    }

    /// <summary>
    /// Provides extension methods for the <see cref="ICommandRouter"/> class.
    /// </summary>
    public static class CommandRouterExtensions
    {
        /// <summary>
        /// Uses the <see cref="ICommandRouter"/> to route a given <see cref="CommandRequest"/> to the corresponding <see cref="ICommand"/>.
        /// </summary>
        /// <param name="router">The <see cref="ICommandRouter"/> to use to retrieve <see cref="ICommand"/> instances.</param>
        /// <param name="request">A <see cref="CommandRequest"/> sent by the app that needs to be handled.</param>
        public static void Execute(this ICommandRouter router, CommandRequest request)
        {
            router.GetCommand(request.Command).Execute(request.Parameter);
        }

        /// <summary>
        /// Retrieves an <see cref="IStream{T}"/> that redirects the <see cref="ICommand.EnabledStream"/> for the command of the given <see cref="CommandInfo"/> description.
        /// </summary>
        /// <param name="router">The <see cref="ICommandRouter"/> to use to retrieve <see cref="ICommand"/> instances.</param>
        /// <param name="command"></param>
        public static IStream<bool> GetEnabled(this ICommandRouter router, CommandInfo command)
        {
            return router.GetCommand(command).EnabledStream;
        }
    }
}
