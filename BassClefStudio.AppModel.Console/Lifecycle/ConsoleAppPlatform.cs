using Autofac;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Provides services and configuration for running <see cref="App"/>s on .NET console apps.
    /// </summary>
    public class ConsoleAppPlatform : IAppPlatform
    {
        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleNavigationService>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<ConsoleStorageService>()
                .AsImplementedInterfaces();
            builder.RegisterType<ConsoleSettingsService>()
                .AsImplementedInterfaces();
            builder.RegisterType<ConsoleDispatcherService>()
                .AsImplementedInterfaces();
        }
    }
}
