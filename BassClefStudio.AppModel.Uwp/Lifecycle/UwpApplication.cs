﻿using BassClefStudio.AppModel.Background;
using BassClefStudio.AppModel.Storage;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// A wrapper class over the <see cref="Windows.UI.Xaml.Application"/>, this class provides a UWP starting-point for running a cross-platform MVVM <see cref="App"/>.
    /// </summary>
    public abstract class UwpApplication : Windows.UI.Xaml.Application
    {
        #region Initialiation

        /// <summary>
        /// Gets the <see cref="UwpApplication"/> object for the current application.
        /// </summary>
        public static new UwpApplication Current => (UwpApplication)Windows.UI.Xaml.Application.Current;

        /// <summary>
        /// The currently attached MVVM <see cref="App"/>.
        /// </summary>
        public App CurrentApp { get; }

        /// <summary>
        /// Creates a new <see cref="UwpApplication"/> object and initializes required resources.
        /// </summary>
        /// <param name="app">The cross-platform app to run in this UWP project.</param>
        /// <param name="viewAssemblies">An array of <see cref="Assembly"/> objects containing all of the <see cref="IView"/>s to register for this app.</param>
        public UwpApplication(App app, params Assembly[] viewAssemblies)
        {
            ////Register system events
            this.Suspending += OnSuspending;

            CurrentApp = app;
            CurrentApp.Initialize(new UwpAppPlatform(), viewAssemblies);
        }

        private bool backHandled = false;
        /// <summary>
        /// Initializes the back navigation event, which the <see cref="IBackHandler"/> uses to handle the back navigation system button.
        /// </summary>
        public void InitializeBackNavigation()
        {
            if(!backHandled)
            {
                SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            }
            backHandled = true;
        }

        #endregion
        #region Events

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            CurrentApp.GoBack();
            e.Handled = true;
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var def = e.SuspendingOperation.GetDeferral();
            CurrentApp.Suspend();
            def.Complete();
        }

        /// <inheritdoc/>
        protected override void OnLaunched(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
        {
            ActivateApp(new LaunchActivatedEventArgs(args.Arguments));
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            ActivateApp(new StorageActivatedEventArgs(args.Files.FirstOrDefault().ToMvvm()));
        }

        /// <inheritdoc/>
        protected override void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            if (args is ILaunchActivatedEventArgs launch)
            {
                ActivateApp(new LaunchActivatedEventArgs(launch.Arguments));
            }
            else if(args is IFileActivatedEventArgs fileArgs)
            {
                ActivateApp(new StorageActivatedEventArgs(fileArgs.Files.FirstOrDefault().ToMvvm()));
            }
            else
            {
                Debug.WriteLine($"Unsupported activation type {args?.GetType().Name}.");
            }
        }

        /// <inheritdoc/>
        protected override void OnBackgroundActivated(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args)
        {
            var task = args.TaskInstance;
            CurrentApp.Activate(
                new BackgroundActivatedEventArgs(
                    task.Task.Name,
                    new UwpBackgroundDeferral(() => task.GetDeferral())));
        }

        #endregion
        #region Activate

        private void ActivateApp(IActivatedEventArgs args)
        {
            CurrentApp.Activate(args);
            InitializeBackNavigation();
        }

        #endregion
    }
}
