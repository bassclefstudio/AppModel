using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
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
    /// Represents a basic <see cref="ICommandRouter"/> that implements the default behaviors.
    /// </summary>
    public class BaseCommandRouter : ICommandRouter
    {
        /// <inheritdoc/>
        public IList<ICommandHandler> ActiveCommandHandlers { get; }

        /// <summary>
        /// Creates a new <see cref="BaseCommandRouter"/>.
        /// </summary>
        public BaseCommandRouter()
        {
            ActiveCommandHandlers = new ObservableCollection<ICommandHandler>();
        }

        /// <inheritdoc/>
        public ICommand GetCommand(CommandInfo command) => ActiveCommandHandlers.GetCommand(command);
    }
}
