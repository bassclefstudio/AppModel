﻿using Autofac;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Diagnostics;
using BassClefStudio.AppModel.Background;
using System.Threading.Tasks;

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
        public ILifetimeScope Services { get; set; }

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
        /// The <see cref="IPackageInfo"/> created by this <see cref="App"/>, containing its name and version info.
        /// </summary>
        public IPackageInfo PackageInfo { get; }

        /// <summary>
        /// Creates a new AppModel <see cref="App"/>.
        /// </summary>
        /// <param name="name">The name of this <see cref="App"/>.</param>
        /// <param name="version">The <see cref="System.Version"/> current version of this <see cref="App"/>.</param>
        public App(string name, Version version)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("An application name must be set.");
            }

            if (version == null)
            {
                throw new ArgumentException("An application version must be present");
            }

            PackageInfo = new PackageInfo() { ApplicationName = name, Version = version };
        }

        /// <summary>
        /// Initializes the default DI <see cref="Services"/> container and runs initialization methods. A shortcut around the <see cref="SetupContainer(ContainerBuilder, IAppPlatform, Assembly[])"/> and <see cref="RunInitMethods"/>.
        /// </summary>
        /// <param name="platform">The app platform that this <see cref="App"/> will use for native services.</param>
        /// <param name="assemblies">The assemblies for this platform that contain any <see cref="IView"/>s or <see cref="IPlatformModule"/>s that the <see cref="App"/> requires.</param>
        public void Initialize(IAppPlatform platform, params Assembly[] assemblies)
        {
            var builder = new ContainerBuilder();
            SetupContainer(builder, platform, assemblies);
            Services = builder.Build();
            RunInitMethods();
        }

        /// <summary>
        /// Sets up an Autofac <see cref="ContainerBuilder"/> with all of the <see cref="App"/> dependencies. Called internally by <see cref="Initialize(IAppPlatform, Assembly[])"/>.
        /// </summary>
        /// <param name="builder">The Autofac <see cref="ContainerBuilder"/> to register services to.</param>
        /// <param name="platform">The app platform that this <see cref="App"/> will use for native services.</param>
        /// <param name="assemblies">The assemblies for this platform that contain any <see cref="IView"/>s or <see cref="IPlatformModule"/>s that the <see cref="App"/> requires.</param>
        public void SetupContainer(ContainerBuilder builder, IAppPlatform platform, params Assembly[] assemblies)
        {
            platform.ConfigureServices(builder);
            //// Register any internal services that deal with lifecycle of the app.
            builder.RegisterAssemblyTypes(typeof(App).Assembly)
                .AssignableTo<ILifecycleHandler>()
                .AsImplementedInterfaces();
            //// Register any IPlatformModules as both modules and types 
            builder.RegisterAssemblyModules<IPlatformModule>(assemblies);
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IPlatformModule>()
                .SingleInstance();
            this.ConfigureServices(builder);
            //// Resister this app instance to all view-models, etc.
            builder.RegisterInstance<App>(this);
            //// If all a service needs is app information (such as name), the IPackageInfo is registered separately.
            builder.RegisterInstance<IPackageInfo>(this.PackageInfo);
            builder.RegisterViews(assemblies);
        }

        /// <summary>
        /// Runs the default initialization methods.
        /// </summary>
        public void RunInitMethods()
        {
            var modules = Services.Resolve<IEnumerable<IPlatformModule>>();
            //// Declare the loaded IPlatformModules
            foreach (var mod in modules)
            {
                Debug.WriteLine($"AppModel: Found module \"{mod.Name}\"");
            }

            //// Run any IInitializationHandlers.
            var inits = Services.Resolve<IEnumerable<IInitializationHandler>>();
            foreach (var i in inits)
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
        #region Activation

        /// <summary>
        /// Starts the <see cref="App"/>, activating the needed services and the UI/view-models.
        /// </summary>
        /// <param name="args">The <see cref="IActivatedEventArgs"/> describing how the <see cref="App"/> was started.</param>
        public void Activate(IActivatedEventArgs args)
        {
            if(args is BackgroundActivatedEventArgs backgroundArgs)
            {
                ActivateBackground(backgroundArgs);
            }
            else
            {
                SynchronousTask registerTask = new SynchronousTask(RegisterBackgroundTasks);
                registerTask.RunTask();
                ActivateForeground(args);
            }
        }

        #region Background

        private void ActivateBackground(BackgroundActivatedEventArgs args)
        {
            var deferral = args.Deferral;
            deferral.StartDeferral();
            try
            {
                IEnumerable<IBackgroundTask> tasks = Services.Resolve<IEnumerable<IBackgroundTask>>();
                IBackgroundTask myTask = tasks.FirstOrDefault(t => t.Id == args.TaskName);
                if (myTask != null)
                {
                    //// Intentionally started without SynchronousTask, in order for the try/catch/finally block can be used to ensure the deferral is executed.
                    _ = RunBackgroundTask(myTask, deferral);
                }
                else
                {
                    Debug.WriteLine($"AppModel: Background task \"{args.TaskName}\" not found.");
                }
            }
            finally
            {
                deferral.EndDeferral();
            }
        }

        private async Task RunBackgroundTask(IBackgroundTask task, IDeferral deferral)
        {
            try
            {
                await task.RunAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"AppModel: Background task \"{task.Id}\" failed: {ex}");
            }
            finally
            {
                deferral.EndDeferral();
            }
        }

        private async Task RegisterBackgroundTasks()
        {
            var service = Services.ResolveOptional<IBackgroundService>();
            if (service != null)
            {
                IEnumerable<IBackgroundTask> tasks = Services.Resolve<IEnumerable<IBackgroundTask>>();
                await service.RegisterCollectionAsync(tasks);
                foreach (var taskName in service.CurrentlyRegistered)
                {
                    Debug.WriteLine($"AppModel: Background task {taskName} registered.");
                }
            }
            else
            {
                Debug.WriteLine("AppModel: Background task service not found. Skipping...");
            }
        }

        #endregion
        #region Foreground

        private void ActivateForeground(IActivatedEventArgs args)
        {
            var navService = Services.Resolve<INavigationService>();
            navService.InitializeNavigation();

            var shellHandler = Services.ResolveOptional<IShellHandler>();
            if (shellHandler != null)
            {
                NavigateReflection(shellHandler);
            }

            var activationHandlers = Services.Resolve<IEnumerable<IActivationHandler>>();
            var activateViewModel = activationHandlers.FirstOrDefault(h => h.CanHandle(args));
            if (activateViewModel != null)
            {
                activateViewModel.Activate(args);
                NavigateReflection(activateViewModel);
            }
            else
            {
                throw new LifecycleException($"No activation service could be found to handle the {args?.GetType().Name} IActivatedEventArgs.");
            }
        }

        #endregion
        #endregion
        #region Navigation

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IViewModel"/> context to navigate to.</typeparam>
        /// <param name="parameter">A parameter passed to the view-model when this <see cref="IViewModel"/> is navigated to.</param>
        public void Navigate<T>(object parameter = null) where T : IViewModel
        {
            var viewModel = Services.Resolve<T>();
            Navigate(viewModel, parameter);
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IViewModel"/> context to navigate to.</typeparam>
        /// <param name="viewModel">A <typeparamref name="T"/> instance of the <see cref="IViewModel"/> to set as the <see cref="IView"/>'s context.</param>
        /// <param name="parameter">A parameter passed to the view-model when this <see cref="IViewModel"/> is navigated to.</param>
        public void Navigate<T>(T viewModel, object parameter = null) where T : IViewModel
        {
            var view = Services.Resolve<IView<T>>();
            var navService = Services.Resolve<INavigationService>();
            navService.Navigate(view);
            view.ViewModel = viewModel;
            NavigatedInitialize(viewModel, view, parameter);
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view. Uses reflection to find the <see cref="IView"/> and <see cref="IViewModel"/> types.
        /// </summary>
        /// <param name="viewModelType">The type of the <see cref="IViewModel"/> to set as the <see cref="IView"/>'s context.</param>
        /// <param name="parameter">A parameter passed to the view-model when this <see cref="IViewModel"/> is navigated to.</param>
        public void NavigateReflection(Type viewModelType, object parameter = null)
        {
            var viewType = typeof(IView<>).MakeGenericType(viewModelType);
            var viewModel = (IViewModel)Services.Resolve(viewModelType);
            var view = (IView)Services.Resolve(viewType);
            var navService = Services.Resolve<INavigationService>();
            navService.Navigate(view);
            viewType.GetProperty("ViewModel").SetValue(view, viewModel);
            NavigatedInitialize(viewModel, view, parameter);
        }

        /// <summary>
        /// Resolves the given <see cref="IViewModel"/>'s view dependencies for the platform and navigates to a new view. Uses reflection to find the <see cref="IView"/> and <see cref="IViewModel"/> types.
        /// </summary>
        /// <param name="viewModel">An instance of the <see cref="IViewModel"/> to set as the <see cref="IView"/>'s context.</param>
        /// <param name="parameter">A parameter passed to the view-model when this <see cref="IViewModel"/> is navigated to.</param>
        public void NavigateReflection(IViewModel viewModel, object parameter = null)
        {
            var viewType = typeof(IView<>).MakeGenericType(viewModel.GetType());
            var view = (IView)Services.Resolve(viewType);
            var navService = Services.Resolve<INavigationService>();
            viewType.GetProperty("ViewModel").SetValue(view, viewModel);
            navService.Navigate(view);
            NavigatedInitialize(viewModel, view, parameter);
        }

        private void NavigatedInitialize(IViewModel viewModel, IView view, object parameter)
        {
            SynchronousTask initViewModelTask =
                new SynchronousTask(() => viewModel.InitializeAsync(parameter));
            initViewModelTask.RunTask();
            view.Initialize();
            var args = new NavigatedEventArgs(view, viewModel, parameter);
            Navigated?.Invoke(this, args);
        }

        /// <summary>
        /// Finishes any tasks to save data and ensure a graceful close of the application (or move to a background process).
        /// </summary>
        public void Suspend()
        {
            var handlers = Services.Resolve<IEnumerable<ISuspendingHandler>>();
            foreach (var h in handlers)
            {
                h.Suspend(this);
            }
        }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="App"/> can initiate back navigation - trigger this by calling the <see cref="GoBack"/> navigation method.
        /// </summary>
        public bool CanGoBack()
        {
            var navService = Services.Resolve<INavigationService>();
            return navService.CanGoBack;
        }

        /// <summary>
        /// Initiates a request to return to the last saved state of the application (i.e. a back button was pressed or gesture detected).
        /// </summary>
        public void GoBack()
        {
            var navService = Services.Resolve<INavigationService>();
            navService.GoBack();
        }

        #endregion
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
                .AssignableTo<IViewModel>()
                .PropertiesAutowired();
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IViewModel>()
                .AsImplementedInterfaces()
                .PropertiesAutowired();
        }

        /// <summary>
        /// Registers the <see cref="IBackgroundTask"/>s in the given <see cref="Assembly"/> instances.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects to find the <see cref="App"/>'s <see cref="IBackgroundTask"/>s.</param>
        public static void RegisterBackgroundTasks(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IBackgroundTask>()
                .AsImplementedInterfaces()
                .SingleInstance();
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
        /// The contextual <see cref="object"/> parameter passed to the <see cref="IViewModel"/>'s <see cref="IViewModel.InitializeAsync(object)"/> method.
        /// </summary>
        public object Parameter { get; }

        /// <summary>
        /// Creates a new <see cref="NavigatedEventArgs"/>.
        /// </summary>
        /// <param name="view">The navigated <see cref="IView"/> view.</param>
        /// <param name="viewModel">The navigated <see cref="IViewModel"/> view-model.</param>
        /// <param name="parameter">The contextual <see cref="object"/> parameter passed to the <see cref="IViewModel"/>'s <see cref="IViewModel.InitializeAsync(object)"/> method.</param>
        public NavigatedEventArgs(IView view, IViewModel viewModel, object parameter)
        {
            NavigatedView = view;
            NavigatedViewModel = viewModel;
            Parameter = parameter;
        }
    }
}
