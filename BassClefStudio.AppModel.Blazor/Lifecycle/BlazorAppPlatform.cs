using Autofac;
using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// The <see cref="IAppPlatform"/> for Blazor WebAssembly apps.
    /// </summary>
    public class BlazorAppPlatform : IAppPlatform
    {
        /// <inheritdoc/>
        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<BlazorNavigationService>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<BlazorViewProvider>()
                .SingleInstance()
                .AsImplementedInterfaces();
            //builder.RegisterType<UwpBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();
            //builder.RegisterType<UwpStorageService>()
            //    .AsImplementedInterfaces();
            //builder.RegisterType<UwpSettingsService>()
            //    .AsImplementedInterfaces();
            //builder.RegisterType<UwpDispatcherService>()
            //    .AsImplementedInterfaces();
            //builder.RegisterType<UwpNotificationService>()
            //    .AsImplementedInterfaces();
        }
    }
}
