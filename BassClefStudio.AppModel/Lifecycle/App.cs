using Autofac;
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
using BassClefStudio.AppModel.Commands;

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
                throw new ArgumentException("An application name must be set.", nameof(name));
            }

            if (version == null)
            {
                throw new ArgumentException("An application version must be present", nameof(version));
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
            Assembly[] fullAssemblies = assemblies
                .Concat(new Assembly[] { typeof(App).Assembly })
                .ToArray();

            platform.ConfigureServices(builder);
            //// Register any internal services that deal with lifecycle of the app.
            builder.RegisterAssemblyTypes(fullAssemblies)
                .AssignableTo<ILifecycleHandler>()
                .AsImplementedInterfaces();
            //// Register any IPlatformModules as both modules and types.
            builder.RegisterAssemblyModules<IPlatformModule>(fullAssemblies);
            builder.RegisterAssemblyTypes(fullAssemblies)
                .AssignableTo<IPlatformModule>()
                .AsImplementedInterfaces()
                .SingleInstance();
            //// Register default navigation components.
            builder.RegisterAssemblyTypes(typeof(App).Assembly)
                .AssignableTo<INavigationHistory>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyTypes(typeof(App).Assembly)
                .AssignableTo<INavigationLayer>()
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(App).Assembly)
                .AssignableTo<INavigationService>()
                .AsImplementedInterfaces()
                .SingleInstance();
            //// Register default commanding components.
            builder.RegisterAssemblyTypes(typeof(App).Assembly)
                .AssignableTo<ICommandRouter>()
                .AsImplementedInterfaces()
                .SingleInstance();
            //// If all a service needs is app information (such as name), the IPackageInfo is registered separately.
            builder.RegisterInstance<IPackageInfo>(this.PackageInfo);
            //// Registers required views.
            builder.RegisterViews(assemblies);

            //// Calls App-specific configuration.
            this.ConfigureServices(builder);
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
                i.Initialize();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Services.Dispose();
        }

        #endregion
        #region Methods
        #region Lifecycle

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

        /// <summary>
        /// Finishes any tasks to save data and ensure a graceful close of the application (or move to a background process).
        /// </summary>
        public void Suspend()
        {
            var handlers = Services.Resolve<IEnumerable<ISuspendingHandler>>();
            foreach (var h in handlers)
            {
                h.Suspend();
            }
        }

        /// <summary>
        /// Gracefully requests back navigation from the default registered <see cref="INavigationService"/>.
        /// </summary>
        public void GoBack()
        {
            var backHandler = Services.Resolve<INavigationService>();
            backHandler.GoBack();
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
            INavigationService navigationController = Services.Resolve<INavigationService>();
            
            IViewProvider viewProvider = Services.Resolve<IViewProvider>();
            viewProvider.StartUI();

            if (Services.IsRegistered<IShellHandler>())
            {
                navigationController.Navigate<IShellHandler>(NavigationProperties.Shell);
            }

            var activationHandler = Services.Resolve<IActivationHandler>();
            var activateViewModel = activationHandler.GetViewModel(args);
            if (activateViewModel != null)
            {
                navigationController.Navigate(activateViewModel, args);
            }
            else
            {
                throw new LifecycleException($"No activation service could be found to handle the {args?.GetType().Name} IActivatedEventArgs.");
            }
        }

        #endregion
        #endregion
        #region Navigation


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
                .PropertiesAutowired()
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
                .PropertiesAutowired()
                .AsImplementedInterfaces();
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

        /// <summary>
        /// Registers any external <see cref="INavigationHistory"/>, <see cref="INavigationLayer"/>, and <see cref="INavigationService"/> services to the DI container, replacing the default ones provided by the system.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects to search for navigation services.</param>
        public static void RegisterExternalNavigation(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            //// Register external navigation components.
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<INavigationHistory>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<INavigationLayer>()
                .AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<INavigationService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        /// <summary>
        /// Registers any external <see cref="ICommandRouter"/> services to the DI container, replacing the default ones provided by the system.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="assemblies">The <see cref="Assembly"/> objects to search for <see cref="ICommandRouter"/>s.</param>
        public static void RegisterExternalCommands(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            //// Register external commanding components.
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<ICommandRouter>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
