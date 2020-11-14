using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.Mvvm.Lifecycle
{
    /// <summary>
    /// Represents a platform on which MVVM apps can be run, providing implementation of abstract services native to the platform.
    /// </summary>
    public interface IAppPlatform
    {
        /// <summary>
        /// Registers the platform-specific implementations of the MVVM services to the DI container.
        /// </summary>
        /// <param name="builder">The <see cref="ContainerBuilder"/> for the dependency injection container.</param>
        void ConfigureServices(ContainerBuilder builder);
    }
}
