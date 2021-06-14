using Autofac;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// The <see cref="IAppPlatform"/> for .NET console apps.
    /// </summary>
    public class ConsoleAppPlatform : IAppPlatform
    {
        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<ConsoleViewProvider>()
                .AsImplementedInterfaces()
                .SingleInstance();
            //builder.RegisterType<ConsoleBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();
            builder.RegisterType<ConsoleStorageService>()
                .AsImplementedInterfaces();
            builder.RegisterType<BaseSettingsService>()
                .AsImplementedInterfaces();
            builder.RegisterType<BaseDispatcher>()
                .WithParameter("dispatcherType", DispatcherType.Main)
                .AsImplementedInterfaces();
            //builder.RegisterType<ConsoleNotificationService>()
            //    .AsImplementedInterfaces();
            builder.RegisterType<LifecycleManager>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
