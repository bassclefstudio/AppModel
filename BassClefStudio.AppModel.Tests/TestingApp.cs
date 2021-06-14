using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Tests
{
    /// <summary>
    /// An <see cref="App"/> specifically designed for testing application core features and dependency injection.
    /// </summary>
    public class TestingApp : App
    {
        private Action<ContainerBuilder> ConfigureAction { get; }

        /// <summary>
        /// Creates a new <see cref="TestingApp"/> with default settings.
        /// </summary>
        public TestingApp() : base(typeof(App).Assembly.GetName().Name, typeof(App).Assembly.GetName().Version)
        {
            ConfigureAction = null;
        }

        /// <summary>
        /// Creates a new <see cref="TestingApp"/> with the given DI configuration.
        /// </summary>
        /// <param name="configure">See <see cref="App.ConfigureServices(ContainerBuilder)"/>.</param>
        public TestingApp(Action<ContainerBuilder> configure) : base(typeof(App).Assembly.GetName().Name, typeof(App).Assembly.GetName().Version)
        {
            ConfigureAction = configure;
        }

        /// <inheritdoc/>
        protected override void ConfigureServices(ContainerBuilder builder)
        {
            if(ConfigureAction != null)
            {
                ConfigureAction(builder);
            }
        }
    }

    public class TestingAppPlatform : IAppPlatform
    {
        private Action<ContainerBuilder> ConfigureAction { get; }

        /// <summary>
        /// Creates a new <see cref="TestingApp"/> with no configuration.
        /// </summary>
        public TestingAppPlatform()
        {
            ConfigureAction = null;
        }

        /// <summary>
        /// Creates a new <see cref="TestingApp"/> with the given DI configuration.
        /// </summary>
        /// <param name="configure">See <see cref="IAppPlatform.ConfigureServices(ContainerBuilder)"/>.</param>
        public TestingAppPlatform(Action<ContainerBuilder> configure)
        {
            ConfigureAction = configure;
        }

        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            if (ConfigureAction != null)
            {
                ConfigureAction(builder);
            }
        }
    }
}
