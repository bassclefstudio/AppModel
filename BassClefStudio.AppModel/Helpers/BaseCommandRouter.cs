using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// Represents a default <see cref="ICommandRouter"/> that populates the available <see cref="ICommand"/>s using <see cref="INavigationHistory"/> for view-models and all resolved <see cref="ICommandProvider"/>s in the app.
    /// </summary>
    public class BaseCommandRouter : ICommandRouter
    {
        /// <summary>
        /// The injected <see cref="INavigationHistory"/> managing currently active <see cref="IViewModel"/>s.
        /// </summary>
        public INavigationHistory NavigationHistory { get; }

        /// <summary>
        /// The injected collection of all <see cref="ICommandProvider"/>s.
        /// </summary>
        public IEnumerable<ICommandProvider> CommandProviders { get; }

        /// <summary>
        /// Creates a new <see cref="BaseCommandRouter"/>.
        /// </summary>
        public BaseCommandRouter(INavigationHistory navigationHistory, IEnumerable<ICommandProvider> commandProviders)
        {
            NavigationHistory = navigationHistory;
            CommandProviders = commandProviders;
        }

        /// <inheritdoc/>
        public ICommand GetCommand(CommandInfo command) => NavigationHistory.GetActiveViewModels().AsEnumerable<ICommandHandler>().Concat(CommandProviders).GetCommand(command);
    }
}
