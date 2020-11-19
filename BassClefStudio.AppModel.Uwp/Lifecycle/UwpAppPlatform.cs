using Autofac;
using BassClefStudio.AppModel.Navigation.Uwp;
using BassClefStudio.AppModel.Settings.Uwp;
using BassClefStudio.AppModel.Storage.Uwp;
using BassClefStudio.AppModel.Threading.Uwp;

namespace BassClefStudio.AppModel.Lifecycle.Uwp
{
    /// <summary>
    /// Provides services and configuration for running <see cref="App"/>s on the UWP platform.
    /// </summary>
    public class UwpAppPlatform : IAppPlatform
    {
        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<UwpNavigationService>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpFileSystemService>()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpSettingsService>()
                .AsImplementedInterfaces();
            builder.RegisterType<UwpDispatcherService>()
                .AsImplementedInterfaces();
        }
    }
}
