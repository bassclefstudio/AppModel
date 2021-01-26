using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Represents a platform-specific, named module, that can inject DI functionality into an AppModel <see cref="App"/>.
    /// </summary>
    public interface IPlatformModule : IModule
    {
        /// <summary>
        /// A unique <see cref="string"/> name that will be registered as the <see cref="App"/> processes its DI services.
        /// </summary>
        string Name { get; }
    }
}
