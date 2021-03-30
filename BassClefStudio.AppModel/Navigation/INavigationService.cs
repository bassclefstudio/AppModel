using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a service that can navigate between <see cref="IView{T}"/>s in a platform-specific way.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Called when the app has been activated with UI, this method should enable the UI, build windows, and start this <see cref="INavigationService"/>'s navigation context.
        /// </summary>
        void InitializeNavigation();

        /// <summary>
        /// Navigates to the given <see cref="IView"/> view, displaying its content to the user.
        /// </summary>
        /// <param name="view">The instance of the <see cref="IView"/> to navigate to.</param>
        void Navigate(IView view);
    }
}
