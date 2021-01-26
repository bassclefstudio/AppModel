using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Autofac.Extensions.DependencyInjection;
using Blazored.LocalStorage;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// A class that deals with the initialization and configuration of a Blazor application that is going to run a cross-platform <see cref="App"/>.
    /// </summary>
    public class BlazorApplication
    {
        /// <summary>
        /// The currently attached MVVM <see cref="App"/>.
        /// </summary>
        public App CurrentApp { get; }

        private Assembly[] PlatformAssemblies { get; }
        /// <summary>
        /// Creates a new <see cref="BlazorApplication"/> object.
        /// </summary>
        /// <param name="app">The cross-platform app to run in this Blazor project.</param>
        /// <param name="platformAssemblies">An array of <see cref="Assembly"/> objects containing all of the <see cref="IView"/>s and <see cref="IPlatformModule"/>s to register for this app.</param>
        public BlazorApplication(App app, params Assembly[] platformAssemblies)
        {
            CurrentApp = app;
            PlatformAssemblies = platformAssemblies;
        }

        /// <summary>
        /// Activates Blazor components, activates the <see cref="CurrentApp"/>, and starts a call to <see cref="WebAssemblyHost.RunAsync"/>.
        /// </summary>
        /// <param name="args"><see cref="string"/> arguments captured by the launch of ths <see cref="BlazorApplication"/>.</param>
        public async Task ActivateAsync<TApp>(string[] args) where TApp : IComponent
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.ConfigureContainer(new AutofacServiceProviderFactory(b => CurrentApp.SetupContainer(b, new BlazorAppPlatform(), PlatformAssemblies)));
            builder.RootComponents.Add<TApp>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredLocalStorage();
            var app = builder.Build();
            CurrentApp.Services = app.Services.GetAutofacRoot();
            await app.RunAsync();
        }
    }
}
