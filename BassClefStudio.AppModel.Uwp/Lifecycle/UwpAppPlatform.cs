using Autofac;
using BassClefStudio.AppModel.Background;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Notifications;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Provides services and configuration for running <see cref="App"/>s on the UWP platform.
    /// </summary>
    public class UwpAppPlatform : IAppPlatform
    {
        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<UwpViewProvider>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpBackgroundService>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpStorageService>()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpSettingsService>()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpDispatcher>()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpNotificationService>()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpLauncher>()
                .AsImplementedInterfaces();
        }
    }
}
