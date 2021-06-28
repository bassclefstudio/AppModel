using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// Represents a default <see cref="ICommandRouter"/> that populates the available <see cref="ICommand"/>s using <see cref="INavigationHistory"/>.
    /// </summary>
    public class BaseCommandRouter : ICommandRouter
    {
        /// <summary>
        /// The injected <see cref="INavigationHistory"/> managing currently active <see cref="IViewModel"/>s.
        /// </summary>
        public INavigationHistory NavigationHistory { get; }

        /// <summary>
        /// Creates a new <see cref="BaseCommandRouter"/>.
        /// </summary>
        public BaseCommandRouter(INavigationHistory navigationHistory)
        {
            NavigationHistory = navigationHistory;
        }

        /// <inheritdoc/>
        public ICommand GetCommand(CommandInfo command) => NavigationHistory.GetActiveViewModels().GetCommand(command);
    }
}
