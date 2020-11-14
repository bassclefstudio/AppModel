using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Mvvm.Lifecycle
{
    /// <summary>
    /// Represents a service that can initialize a particular service when the <see cref="App"/> initializes.
    /// </summary>
    public interface IInitializationHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called when the <see cref="App"/> initializes.
        /// </summary>
        /// <param name="app">The app's <see cref="App"/> object.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool Initialize(App app);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="App"/>'s back navigation.
    /// </summary>
    public interface IBackHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever the <see cref="App"/> receieves a back navigation request.
        /// </summary>
        /// <param name="app">The app's <see cref="App"/> object.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool BackRequested(App app);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="App"/>'s suspension from the foreground.
    /// </summary>
    public interface ISuspendingHandler : ILifecycleHandler
    {
        /// <summary>
        /// A method that is called whenever a foreground-activated <see cref="App"/> is closing or returning to the background. Here, the <see cref="ILifecycleHandler"/> can dispose or broker resources before the <see cref="App"/> has fully closed.
        /// </summary>
        /// <param name="app">The app's <see cref="App"/> object.</param>
        /// <returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool OnSuspending(App app);
    }

    /// <summary>
    /// Represents a service that can manage an <see cref="App"/>'s foreground activation.
    /// </summary>
    public interface IActivationHandler : ILifecycleHandler
    {
        /// <summary>
        /// Returns a <see cref="bool"/> indicating whether this <see cref="IActivationHandler"/> can handle the given <see cref="IActivatedEventArgs"/>.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> arguments to handle.</param>
        bool CanHandle(IActivatedEventArgs args);

        /// <summary>
        /// A method that is called whenever the <see cref="App"/> is activated in the foreground - i.e., by a user launching the App, through a file or URI scheme, or the like.
        /// </summary>
        /// <param name="app">The app's <see cref="App"/> object.</param>
        /// <param name="args">Represents the </param>
        ///<returns>A <see cref="bool"/> value indicating whether any action was performed successfully.</returns>
        bool Activated(App app, IActivatedEventArgs args);
    }

    /// <summary>
    /// Represents a service that can handle lifecycle events for an <see cref="App"/>.
    /// </summary>
    public interface ILifecycleHandler
    {
        /// <summary>
        /// A <see cref="bool"/> value indicating whether the <see cref="ILifecycleHandler"/> is enabled.
        /// </summary>
        bool Enabled { get; }
    }
}
