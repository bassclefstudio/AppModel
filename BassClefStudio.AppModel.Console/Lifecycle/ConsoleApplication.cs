using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.AppModel.Navigation;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// A class that deals with the initialization and configuration of a .NET console application that is going to run a cross-platform <see cref="App"/>.
    /// </summary>
    public class ConsoleApplication
    {
        /// <summary>
        /// The currently attached MVVM <see cref="App"/>.
        /// </summary>
        public App CurrentApp { get; }

        /// <summary>
        /// Creates a new <see cref="BlazorApplication"/> object.
        /// </summary>
        /// <param name="app">The cross-platform app to run in this console project.</param>
        /// <param name="platformAssemblies">An array of <see cref="Assembly"/> objects containing all of the <see cref="IView"/>s and <see cref="IPlatformModule"/>s to register for this app.</param>
        public ConsoleApplication(App app, params Assembly[] platformAssemblies)
        {
            CurrentApp = app;
            CurrentApp.Initialize(new ConsoleAppPlatform(), platformAssemblies);
        }

        /// <summary>
        /// Starts and activates the <see cref="CurrentApp"/>, and waits until the app is suspended before completing.
        /// </summary>
        public async Task StartApplicationAsync(string[] args)
        {
            CurrentApp.Activate(new LaunchActivatedEventArgs(args));
            await LifecycleManager.ApplicationTask;
        }
    }
}
