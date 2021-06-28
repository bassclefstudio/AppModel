using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// Any service that provides a set of <see cref="ICommand"/> commands that it handles.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// The collection of <see cref="ICommand"/>s that this <see cref="ICommandHandler"/> provides.
        /// </summary>
        IList<ICommand> Commands { get; }
    }

    /// <summary>
    /// An <see cref="ICommandHandler"/> that's not attached to an active view-model, but instad provides app-wide command definitions.
    /// </summary>
    public interface ICommandProvider : ICommandHandler
    { }

    /// <summary>
    /// Provides extension methods for the <see cref="ICommandHandler"/> interface.
    /// </summary>
    public static class CommandHandlerExtensions
    {
        /// <summary>
        /// Gets the <see cref="ICommand"/> matching the <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="handlers">The <see cref="ICommandHandler"/>s containing the collection of available commands.</param>
        /// <param name="info">The <see cref="CommandInfo"/> info to match.</param>
        public static ICommand GetCommand(this IEnumerable<ICommandHandler> handlers, CommandInfo info) => GetCommand(handlers.SelectMany(h => h.Commands), info);
        /// <summary>
        /// Gets the <see cref="ICommand"/> matching the <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="handler">The <see cref="ICommandHandler"/> containing the collection of available commands.</param>
        /// <param name="info">The <see cref="CommandInfo"/> info to match.</param>
        public static ICommand GetCommand(this ICommandHandler handler, CommandInfo info) => GetCommand(handler.Commands, info);
        /// <summary>
        /// Gets the <see cref="ICommand"/> matching the <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="commands">The collection of available commands.</param>
        /// <param name="info">The <see cref="CommandInfo"/> info to match.</param>
        public static ICommand GetCommand(this IEnumerable<ICommand> commands, CommandInfo info)
        {
            return commands.FirstOrDefault(c => c.Info == info);
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> matching the <see cref="string"/> ID.
        /// </summary>
        /// <param name="handlers">The <see cref="ICommandHandler"/>s containing the collection of available commands.</param>
        /// <param name="id">The <see cref="string"/> desired ID of the command.</param>
        public static ICommand GetCommand(this IEnumerable<ICommandHandler> handlers, string id) => GetCommand(handlers.SelectMany(h => h.Commands), id);
        /// <summary>
        /// Gets the <see cref="ICommand"/> matching the <see cref="string"/> ID.
        /// </summary>
        /// <param name="handler">The <see cref="ICommandHandler"/> containing the collection of available commands.</param>
        /// <param name="id">The <see cref="string"/> desired ID of the command.</param>
        public static ICommand GetCommand(this ICommandHandler handler, string id) => GetCommand(handler.Commands, id);
        /// <summary>
        /// Gets the <see cref="ICommand"/> matching the <see cref="string"/> ID.
        /// </summary>
        /// <param name="commands">The collection of available commands.</param>
        /// <param name="id">The <see cref="string"/> desired ID of the command.</param>
        public static ICommand GetCommand(this IEnumerable<ICommand> commands, string id)
        {
            return commands.FirstOrDefault(c => c.Info.Id == id);
        }
    }
}
