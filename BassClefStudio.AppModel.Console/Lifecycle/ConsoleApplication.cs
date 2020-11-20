using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    public class ConsoleApplication
    {
        /// <summary>
        /// The currently attached MVVM <see cref="App"/>.
        /// </summary>
        public App CurrentApp { get; }

        /// <summary>
        /// Creates a new <see cref="ConsoleApplication"/> object and initializes required resources.
        /// </summary>
        /// <param name="app">The cross-platform app to run in this .NET console project.</param>
        public ConsoleApplication(App app, params Assembly[] viewAssemblies)
        {
            CurrentApp = app;
            CurrentApp.Initialize(new ConsoleAppPlatform(), viewAssemblies);
        }

        /// <summary>
        /// Creates the <see cref="IActivatedEventArgs"/> from the recieved console arguments.
        /// </summary>
        /// <param name="args">The <see cref="string"/> array of parameters.</param>
        protected virtual IActivatedEventArgs CreateEventArgs(string[] args)
        {
            return new LaunchActivatedEventArgs(args);
        }

        public void Start(string[] args)
        {
            CurrentApp.Activate(CreateEventArgs(args));
            while(!IsSuspended)
            { }
            CurrentApp.Suspend();
        }

        protected bool IsSuspended { get; private set; } = false;
        public void Suspend()
        {
            IsSuspended = true;
        }
    }
}
