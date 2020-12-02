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
    public class WpfAppPlatform : IAppPlatform
    {
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
            builder.RegisterType<WpfDispatcherService>()
                .AsImplementedInterfaces();
            //builder.RegisterType<WpfNotificationService>()
            //    .AsImplementedInterfaces();
        }
    }
}
