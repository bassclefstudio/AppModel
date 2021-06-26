using BassClefStudio.AppModel.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Provides a collection of all the active <see cref="IViewModel"/>s that are currently handling parts of the app, which can be used to populate systems that manage things such as <see cref="ICommandHandler"/>s.
    /// </summary>
    public interface INavigationActiveHandler
    {
        /// <summary>
        /// Gets a current collection of all <see cref="IViewModel"/>s which currently manage parts of the active app surface (this includes currently-navigated pages, secondary windows, and 'shell' pages).
        /// </summary>
        IEnumerable<IViewModel> ActiveViewModels { get; }

        /// <summary>
        /// Adds the new <see cref="IViewModel"/> to the <see cref="ActiveViewModels"/> collection using the provided <see cref="NavigationMode"/>.
        /// </summary>
        /// <param name="viewModel">The <see cref="IViewModel"/> instance to add.</param>
        /// <param name="mode">The <see cref="NavigationMode"/> describing how this <paramref name="viewModel"/> affects previously-navigated view-models.</param>
        void Add(IViewModel viewModel, NavigationMode mode);
    }
}
