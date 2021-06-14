using Autofac;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Threading;
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
            builder.RegisterType<BlazorViewProvider>()
                .SingleInstance()
                .AsImplementedInterfaces();
            builder.RegisterType<BlazorViewInfo>()
                .SingleInstance()
                .AsImplementedInterfaces();
            //builder.RegisterType<BlazorBackgroundService>()
            //    .SingleInstance()
            //    .AsImplementedInterfaces();
            //builder.RegisterType<BlazorStorageService>()
            //    .AsImplementedInterfaces();
            builder.RegisterType<BlazorSettingsService>()
                .AsImplementedInterfaces();
            builder.RegisterType<BaseDispatcher>()
                .WithParameter("dispatcherType", DispatcherType.Main)
                .AsImplementedInterfaces();
            //builder.RegisterType<BlazorNotificationService>()
            //    .AsImplementedInterfaces();
        }
    }
}
