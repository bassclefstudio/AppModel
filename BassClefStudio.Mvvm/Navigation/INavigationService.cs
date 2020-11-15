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
        /// Navigates to the given <see cref="IView"/> view, displaying its content to the user.
        /// </summary>
        /// <param name="view">The instance of the <see cref="IView"/> to navigate to.</param>
        /// <param name="parameter">An <see cref="object"/> parameter that can be passed to the view on navigation.</param>
        void Navigate(IView view, object parameter = null);

        /// <summary>
        /// An event fired when the <see cref="INavigationService"/> completes successful navigation to a new <see cref="IView"/>.
        /// </summary>
        event EventHandler Navigated;
    }
}
