using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Represents a service that can initialize a particular service when the <see cref="App"/> initializes.
    /// </summary>
    public interface IInitializationHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called when the <see cref="App"/> initializes.
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool Initialize();
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="App"/>'s suspension from the foreground.
    /// </summary>
    public interface ISuspendingHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever a foreground-activated <see cref="App"/> is closing or returning to the background. Here, the <see cref="ILifecycleHandler"/> can dispose or broker resources before the <see cref="App"/> has fully closed.
        /// </summary>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool Suspend();
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="App"/>'s foreground activation.
    /// </summary>
    public interface IActivationHandler : ILifecycleHandler
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of view-model needed to handle the provided <see cref="IActivatedEventArgs"/>.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> arguments passed to the <see cref="App"/> when it was launched.</param>
        /// <returns>The <see cref="Type"/> of the view-model that can handle these arguments, or 'null' if none is found.</returns>
        Type GetViewModel(IActivatedEventArgs args);
    }

    /// <summary>
    /// Represents a view-model that will be navigated to when the app is initialized, for it to provide additional navigation 'chrome'.
    /// </summary>
    public interface IShellHandler : ILifecycleHandler, IViewModel
    { }

    /// <summary>
    /// Represents a service that can handle lifecycle events for an <see cref="App"/>.
    /// </summary>
    public interface ILifecycleHandler
    { }
}
