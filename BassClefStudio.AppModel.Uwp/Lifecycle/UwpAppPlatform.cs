﻿using Autofac;
using BassClefStudio.AppModel.Navigation;
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
