using Autofac;
using BassClefStudio.Mvvm.Lifecycle;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.Mvvm.Navigation
{
    /// <summary>
    /// Represents a service that can navigate between <see cref="IView{T}"/>s.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to the given <typeparamref name="T"/> view and initializes the view-model.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="IView"/> to initialize and navigate to.</typeparam>
        /// <param name="parameter">An <see cref="object"/> parameter that can be passed to the view on initialization.</param>
        void Navigate<T>(object parameter = null) where T : IView;

        /// <summary>
        /// An event fired when the <see cref="INavigationService"/> completes successful navigation to a new <see cref="IView"/>.
        /// </summary>
        event EventHandler<NavigatedEventArgs> Navigated;
    }

    /// <summary>
    /// An <see cref="EventArgs"/> passed by the <see cref="INavigationService"/> on successful navigation.
    /// </summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>
        /// The content of the currently-navigated view.
        /// </summary>
        public IView NavigatedPage { get; }

        /// <summary>
        /// Creates a new <see cref="NavigatedEventArgs"/>.
        /// </summary>
        /// <param name="navigatedPage">The <see cref="IView"/> that the <see cref="INavigationService"/> just completed navigation to.</param>
        public NavigatedEventArgs(IView navigatedPage)
        {
            NavigatedPage = navigatedPage;
        }
    }
}
