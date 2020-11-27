using Autofac;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace BassClefStudio.AppModel.Lifecycle
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
        /// An event fired when the <see cref="App"/> completes successful navigation to a new <see cref="IViewModel"/> and its associated <see cref="IView"/>.
        /// </summary>
        public event EventHandler<NavigatedEventArgs> Navigated;

        /// <summary>
        /// Configures any app-specific services, such as registering of views, view-models, and app-specific behaviors and services.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> for the dependency injection container.</param>
        protected abstract void ConfigureServices(ContainerBuilder builder);

        /// <summary>
        /// Initializes the DI <see cref="Services"/> container.
        /// </summary>
        /// <param name="platform">The app platform that this <see cref="App"/> will use for native services.</param>
        /// <param name="assemblies">The assemblies for this platform that contain any <see cref="IView"/>s that the <see cref="App"/> requires.</param>
        public void Initialize(IAppPlatform platform, params Assembly[] assemblies)
        {
            var builder = new ContainerBuilder();
            platform.ConfigureServices(builder);
            this.ConfigureServices(builder);
            //// Resister this app instance to all view-models, etc.
            builder.RegisterInstance<App>(this);
            builder.RegisterViews(assemblies);

            Services = builder.Build();

            //// Run any IInitializationHandlers.
            var inits = Services.Resolve<IEnumerable<IInitializationHandler>>();
            foreach (var i in inits.Where(s => s.Enabled))
            {
                i.Initialize(this);
            }
        }

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
        /// <param name="args">The <see cref="IActivatedEventArgs"/> describing how the <see cref="App"/> was started.</param>
        public void Activate(IActivatedEventArgs args)
        {
            var navService = Services.Resolve<INavigationService>();
            navService.InitializeNavigation();

            var shellHandlers = Services.Resolve<IEnumerable<IShellHandler>>();
            var shellHandler = shellHandlers.FirstOrDefault(h => h.Enabled);
            if (shellHandler != null)
            {
                NavigateReflection(shellHandler);
            }

            var activationHandlers = Services.Resolve<IEnumerable<IActivationHandler>>();
            var activateViewModel = activationHandlers.Where(h => h.Enabled).FirstOrDefault(h => h.CanHandle(args));
            if(activateViewModel != null)
            {
                activateViewModel.Activate(args);
                NavigateReflection(activateViewModel);
            }
            else
            {
                throw new LifecycleException($"No activation service could be found to handle the {args?.GetType().Name} IActivatedEventArgs.");
            }
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IViewModel"/> context to navigate to.</typeparam>
        public void Navigate<T>() where T : IViewModel
        {
            var viewModel = Services.Resolve<T>();
            Navigate(viewModel);
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IViewModel"/> context to navigate to.</typeparam>
        /// <param name="viewModel">A <typeparamref name="T"/> instance of the <see cref="IViewModel"/> to set as the <see cref="IView"/>'s context.</param>
        public void Navigate<T>(T viewModel) where T : IViewModel
        {
            var view = Services.Resolve<IView<T>>();
            var navService = Services.Resolve<INavigationService>();
            navService.Navigate(view);
            view.ViewModel = viewModel;
            NavigatedInitialize(viewModel, view);
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view. Uses reflection to find the <see cref="IView"/> and <see cref="IViewModel"/> types.
        /// </summary>
        /// <param name="viewModelType">The type of the <see cref="IViewModel"/> to set as the <see cref="IView"/>'s context.</param>
        public void NavigateReflection(Type viewModelType)
        {
            var viewType = typeof(IView<>).MakeGenericType(viewModelType);
            var viewModel = (IViewModel)Services.Resolve(viewModelType);
            var view = (IView)Services.Resolve(viewType);
            var navService = Services.Resolve<INavigationService>();
            navService.Navigate(view);
            viewType.GetProperty("ViewModel").SetValue(view, viewModel);
            NavigatedInitialize(viewModel, view);
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view. Uses reflection to find the <see cref="IView"/> and <see cref="IViewModel"/> types.
        /// </summary>
        /// <param name="viewModel">An instance of the <see cref="IViewModel"/> to set as the <see cref="IView"/>'s context.</param>
        public void NavigateReflection(IViewModel viewModel)
        {
            var viewType = typeof(IView<>).MakeGenericType(viewModel.GetType());
            var view = (IView)Services.Resolve(viewType);
            var navService = Services.Resolve<INavigationService>();
            navService.Navigate(view);
            viewType.GetProperty("ViewModel").SetValue(view, viewModel);
            NavigatedInitialize(viewModel, view);
        }

        private void NavigatedInitialize(IViewModel viewModel, IView view)
        {
            SynchronousTask initViewModelTask =
                new SynchronousTask(viewModel.InitializeAsync);
            initViewModelTask.RunTask();
            view.Initialize();
            Navigated?.Invoke(this, new NavigatedEventArgs(view, viewModel));
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
        /// Registers the <see cref="IView"/>s in the given <see cref="Assembly"/> instances.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects to find the <see cref="App"/>'s <see cref="IView"/>s.</param>
        public static void RegisterViews(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IView>()
                .AsImplementedInterfaces();
        }

        /// <summary>
        /// Registers the <see cref="IViewModel"/>s in the given <see cref="Assembly"/> instances.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects to find the <see cref="App"/>'s <see cref="IViewModel"/>s.</param>
        public static void RegisterViewModels(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IViewModel>();
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IViewModel>()
                .AsImplementedInterfaces();
        }
    }

    /// <summary>
    /// Event information showing the resolved <see cref="IViewModel"/> and <see cref="IView"/> of a successful navigation.
    /// </summary>
    public class NavigatedEventArgs : EventArgs
    {
        /// <summary>
        /// The navigated <see cref="IView"/> view.
        /// </summary>
        public IView NavigatedView { get; }

        /// <summary>
        /// The navigated <see cref="IViewModel"/> view-model.
        /// </summary>
        public IViewModel NavigatedViewModel { get; }

        /// <summary>
        /// Creates a new <see cref="NavigatedEventArgs"/>.
        /// </summary>
        /// <param name="view">The navigated <see cref="IView"/> view.</param>
        /// <param name="viewModel">The navigated <see cref="IViewModel"/> view-model.</param>
        public NavigatedEventArgs(IView view, IViewModel viewModel)
        {
            NavigatedView = view;
            NavigatedViewModel = viewModel;
        }
    }
}
