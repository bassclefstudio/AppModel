using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// A wrapper class over the <see cref="System.Windows.Application"/>, this class provides a WPF starting-point for running a cross-platform MVVM <see cref="App"/>.
    /// </summary>
    public class WpfApplication : Application
    {
        /// <summary>
        /// The currently attached MVVM <see cref="App"/>.
        /// </summary>
        public App CurrentApp { get; }

        /// <summary>
        /// Creates a new <see cref="WpfApplication"/> object and initializes required resources.
        /// </summary>
        /// <param name="app">The cross-platform app to run in this WPF project.</param>
        /// <param name="platformAssemblies">An array of <see cref="Assembly"/> objects containing all of the <see cref="IView"/>s and <see cref="IPlatformModule"/>s to register for this app.</param>
        public WpfApplication(App app, params Assembly[] platformAssemblies)
        {
            CurrentApp = app;
            CurrentApp.Initialize(new WpfAppPlatform(), platformAssemblies);
            this.Startup += AppStarting;
        }

        private void AppStarting(object sender, StartupEventArgs e)
        {
            CurrentApp.Activate(new LaunchActivatedEventArgs(e.Args));
        }
    }
}
