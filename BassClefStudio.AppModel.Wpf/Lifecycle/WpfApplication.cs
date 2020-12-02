using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BassClefStudio.AppModel.Wpf
{
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
        /// <param name="viewAssemblies">An array of <see cref="Assembly"/> objects containing all of the <see cref="IView"/>s to register for this app.</param>
        public WpfApplication(App app, params Assembly[] viewAssemblies)
        {
            CurrentApp = app;
            CurrentApp.Initialize(new WpfAppPlatform(), viewAssemblies);
        }

        protected override void OnActivated(EventArgs e)
        {
            CurrentApp.Activate(new LaunchActivatedEventArgs());
            base.OnActivated(e);
        }
    }
}
