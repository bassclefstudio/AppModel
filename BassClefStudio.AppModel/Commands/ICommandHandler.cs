using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// A service (usually an <see cref="IViewModel"/>) that provides a set of <see cref="ICommand{T}"/> commands that it handles.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// The collection of <see cref="ICommand"/>s that this <see cref="ICommandHandler"/> provides.
        /// </summary>
        ICommand[] Commands { get; }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="ICommandHandler"/> interface.
    /// </summary>
    public static class CommandHandlerExtensions
    {
        /// <summary>
        /// Attempts to execute the <see cref="ICommand"/> matching the <see cref="CommandInfo"/> with the provided input.
        /// </summary>
        /// <param name="handler">The <see cref="ICommandHandler"/> with the collection of available commands.</param>
        /// <param name="info">The <see cref="CommandInfo"/> info to match.</param>
        /// <param name="input">The optional <see cref="object"/> input to pass to the command.</param>
        public static void ExecuteCommand(this ICommandHandler handler, CommandInfo info, object input = null)
        {
            var command = handler.Commands.FirstOrDefault(c => c.Info == info);
            if(command != null)
            {
                command.InitiateCommand(input);
            }
            else
            {
                throw new ArgumentException($"Command matching {info} was not found.", "info");
            }
        }

        /// <summary>
        /// Attempts to execute the <see cref="ICommand"/> matching the <see cref="CommandInfo"/> with the provided input.
        /// </summary>
        /// <param name="handler">The <see cref="ICommandHandler"/> with the collection of available commands.</param>
        /// <param name="id">The <see cref="string"/> desired ID of the command.</param>
        /// <param name="input">The optional <see cref="object"/> input to pass to the command.</param>
        public static void ExecuteCommand(this ICommandHandler handler, string id, object input = null)
        {
            var command = handler.Commands.FirstOrDefault(c => c.Info.Id == id);
            if (command != null)
            {
                command.InitiateCommand(input);
            }
            else
            {
                throw new ArgumentException($"Command matching {id} was not found.", "id");
            }
        }

        /// <summary>
        /// Attempts to execute the <see cref="ICommand{T}"/> matching the <see cref="CommandInfo"/> with the provided input.
        /// </summary>
        /// <param name="handler">The <see cref="ICommandHandler"/> with the collection of available commands.</param>
        /// <param name="info">The <see cref="CommandInfo"/> info to match.</param>
        /// <param name="input">The optional <typeparamref name="T"/> input to pass to the command.</param>
        /// <typeparam name="T">The type of <see cref="ICommand{T}"/> to filter for in the <paramref name="handler"/>.</typeparam>
        public static void ExecuteCommand<T>(this ICommandHandler handler, CommandInfo info, T input = default(T))
        {
            var command = handler.Commands.OfType<ICommand<T>>().FirstOrDefault(c => c.Info == info);
            if (command != null)
            {
                command.InitiateCommand(input);
            }
            else
            {
                throw new ArgumentException($"Command matching {info} was not found.", "info");
            }
        }

        /// <summary>
        /// Attempts to execute the <see cref="ICommand{T}"/> matching the <see cref="CommandInfo"/> with the provided input.
        /// </summary>
        /// <param name="handler">The <see cref="ICommandHandler"/> with the collection of available commands.</param>
        /// <param name="id">The <see cref="string"/> desired ID of the command.</param>
        /// <param name="input">The optional <typeparamref name="T"/> input to pass to the command.</param>
        /// <typeparam name="T">The type of <see cref="ICommand{T}"/> to filter for in the <paramref name="handler"/>.</typeparam>
        public static void ExecuteCommand<T>(this ICommandHandler handler, string id, T input = default(T))
        {
            var command = handler.Commands.OfType<ICommand<T>>().FirstOrDefault(c => c.Info.Id == id);
            if (command != null)
            {
                command.InitiateCommand(input);
            }
            else
            {
                throw new ArgumentException($"Command matching {id} was not found.", "id");
            }
        }
    }
}
