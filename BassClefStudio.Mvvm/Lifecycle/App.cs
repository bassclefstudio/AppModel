using Autofac;
using BassClefStudio.Mvvm.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace BassClefStudio.Mvvm.Lifecycle
{
    /// <summary>
    /// Represents a platform-agnostic application that uses the native services provided by an <see cref="IAppPlatform"/> to create views and manage app lifecycle.
    /// </summary>
    public abstract class App : IDisposable
    {
        #region Services

        /// <summary>
        /// The Autofac services container used for resolving services in the <see cref="App"/>.
        /// </summary>
        public IContainer Services { get; private set; }

        /// <summary>
        /// Configures any app-specific services, such as registering of views, view-models, and app-specific behaviors and services.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> for the dependency injection container.</param>
        protected abstract void ConfigureServices(ContainerBuilder builder);

        /// <summary>
        /// Initializes the DI <see cref="Services"/> container.
        /// </summary>
        /// <param name="platform">The app platform that this <see cref="App"/> will use for native services.</param>
        public void Initialize(IAppPlatform platform)
        {
            var builder = new ContainerBuilder();
            platform.ConfigureServices(builder);
            this.ConfigureServices(builder);
            Services = builder.Build();

            //// Initialize navigation DI.
            InitializeNavigation();
            //// Run any IInitializationHandlers.
            var inits = Services.Resolve<IEnumerable<IInitializationHandler>>();
            foreach (var i in inits.Where(s => s.Enabled))
            {
                i.Initialize(this);
            }
        }

        #region Navigation

        /// <summary>
        /// Starts the navigation stack.
        /// </summary>
        private void InitializeNavigation()
        {
            INavigationService navigationService = Services.Resolve<INavigationService>();
            if (navigationService != null)
            {
                navigationService.Navigated += ViewChanged;
            }
            else
            {
                throw new LifecycleException("Could not initialize the MVVM application because no INavigaionService was registered in the services container.");
            }
        }

        private void ViewChanged(object sender, NavigatedEventArgs e)
        {
            Services.InjectProperties(e.NavigatedPage);
            SynchronousTask initTask = new SynchronousTask(
                e.NavigatedPage.InitializeAsync);
            initTask.RunTask();
        }

        #endregion

        /// <inheritdoc/>
        public void Dispose()
        {
            Services.Dispose();
        }

        #endregion
        #region Methods

        /// <summary>
        /// Starts the <see cref="App"/>, activating the needed services and the UI/view-models.
        /// </summary>
        /// <param name="args"></param>
        public void Activate(IActivatedEventArgs args)
        {
            var handlers = Services.Resolve<IEnumerable<IActivationHandler>>();
            var myHandler = handlers.Where(h => h.Enabled).FirstOrDefault(h => h.CanHandle(args));
            if(myHandler != null)
            {
                myHandler.Activate(this, args);
            }
            else
            {
                throw new LifecycleException($"No activation service could be found to handle the {args?.GetType().Name} IActivatedEventArgs.");
            }
        }

        /// <summary>
        /// Finishes any tasks to save data and ensure a graceful close of the application (or move to a background process).
        /// </summary>
        public void Suspend()
        {
            var handlers = Services.Resolve<IEnumerable<ISuspendingHandler>>();
            foreach (var h in handlers.Where(s => s.Enabled))
            {
                h.Suspend(this);
            }
        }

        /// <summary>
        /// Initiates a request to return to the last saved state of the application (i.e. a back button was pressed or gesture detected).
        /// </summary>
        public void GoBack()
        {
            var handlers = Services.Resolve<IEnumerable<IBackHandler>>();
            var myHandler = handlers.Where(h => h.Enabled).FirstOrDefault();
            if (myHandler != null)
            {
                myHandler.GoBack(this);
            }
            else
            {
                throw new LifecycleException($"No enabled IBackHandlers could be found in the services container for the App.");
            }
        }

        #endregion
    }

    /// <summary>
    /// An <see cref="Exception"/> thrown if an issue is found setting up or during the execution of an <see cref="App"/> or its <see cref="ILifecycleHandler"/>s.
    /// </summary>
    public class LifecycleException : Exception
    {
        /// <inheritdoc/>
        public LifecycleException() { }
        /// <inheritdoc/>
        public LifecycleException(string message) : base(message) { }
        /// <inheritdoc/>
        public LifecycleException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Extension methods for the Autofac <see cref="ContainerBuilder"/> for adding MVVM services to the container.
    /// </summary>
    public static class DIExtensions
    {
        /// <summary>
        /// Adds all <see cref="IViewModel"/>s and <see cref="ILifecycleHandler"/>s in the provided assemblies to the given Autofac <see cref="ContainerBuilder"/> so they can be accessed and resolved by the running <see cref="App"/>.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> registering services for the <see cref="App"/>.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> instances where to look for <see cref="IViewModel"/>s.</param>
        public static void AddMvvm(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IViewModel>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<ILifecycleHandler>()
                .AsImplementedInterfaces();
        }
    }
}
