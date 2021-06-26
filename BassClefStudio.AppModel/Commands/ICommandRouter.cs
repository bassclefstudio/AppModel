using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BassClefStudio.AppModel.Commands
{
    /// <summary>
    /// A service responsible for determining the active <see cref="ICommandHandler"/>s in a given app retrieving the relevant <see cref="ICommand"/> instances from them. 
    /// </summary>
    public interface ICommandRouter
    {
        /// <summary>
        /// Retrieves the current <see cref="ICommand"/> instance routed to the given <see cref="CommandInfo"/> command.
        /// </summary>
        /// <param name="command">The <see cref="CommandInfo"/> command definition.</param>
        /// <returns>An <see cref="ICommand"/> instance of the desired command.</returns>
        ICommand GetCommand(CommandInfo command);
    }

    /// <summary>
    /// Provides a singleton <see cref="ICommandRouter"/> instance to views and controls.
    /// </summary>
    public class CommandRouter : IInitializationHandler
    {
        /// <summary>
        /// The currently exposed <see cref="ICommandRouter"/> router.
        /// </summary>
        public static ICommandRouter Instance { get; internal set; }

        /// <inheritdoc cref="CommandRouterExtensions.Execute(ICommandRouter, CommandRequest)"/>
        public static void Execute(CommandRequest request) => Instance.Execute(request);

        /// <summary>
        /// Uses the <see cref="ICommandRouter"/> to route a given <see cref="CommandRequest"/> to the corresponding <see cref="ICommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="CommandInfo"/> command to execute.</param>
        /// <param name="parameter">An optional input to pass to the command.</param>
        public static void Execute(CommandInfo command, object parameter = null) => Instance.Execute(new CommandRequest() { Command = command, Parameter = parameter });

        /// <inheritdoc cref="CommandRouterExtensions.GetEnabled(ICommandRouter, CommandInfo)"/>
        public static IStream<bool> GetEnabled(CommandInfo command) => Instance.GetEnabled(command);

        /// <summary>
        /// Creates a new <see cref="CommandRouter"/> with the desired <see cref="ICommandRouter"/> singleton.
        /// </summary>
        public CommandRouter(ICommandRouter instance)
        {
            Instance = instance;
        }

        /// <inheritdoc/>
        public bool Initialize()
        {
            return Instance != null;
        }
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
