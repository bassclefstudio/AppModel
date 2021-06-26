using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// Represents a basic <see cref="ICommandRouter"/> that populates the available <see cref="ICommand"/>s using the <see cref="INavigationActiveHandler"/> service.
    /// </summary>
    public class BaseCommandRouter : ICommandRouter
    {
        /// <summary>
        /// The injected <see cref="INavigationActiveHandler"/> managing currently active <see cref="IViewModel"/>s.
        /// </summary>
        public INavigationActiveHandler NavigationActiveHandler { get; }

        /// <summary>
        /// Creates a new <see cref="BaseCommandRouter"/>.
        /// </summary>
        public BaseCommandRouter(INavigationActiveHandler navigationActiveHandler)
        {
            NavigationActiveHandler = navigationActiveHandler;
        }

        /// <inheritdoc/>
        public ICommand GetCommand(CommandInfo command) => NavigationActiveHandler.ActiveViewModels.GetCommand(command);
    }
}
