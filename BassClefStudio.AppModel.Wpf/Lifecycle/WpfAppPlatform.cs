using Autofac;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// The <see cref="IAppPlatform"/> for WPF apps.
    /// </summary>
    public class WpfAppPlatform : IAppPlatform
    {
        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<WpfNavigationService>()
                .SingleInstance()
                .AsImplementedInterfaces();
            //builder.RegisterType<WpfBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();
            builder.RegisterType<WpfStorageService>()
                .AsImplementedInterfaces();
            builder.RegisterType<BaseSettingsService>()
                .AsImplementedInterfaces();
            builder.RegisterType<WpfDispatcher>()
                .AsImplementedInterfaces();
            //builder.RegisterType<WpfNotificationService>()
            //    .AsImplementedInterfaces();
            builder.RegisterType<WpfLauncher>()
                .AsImplementedInterfaces();
        }
    }
}
