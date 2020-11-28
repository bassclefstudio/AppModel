using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// Represents a service that can provide a task that this <see cref="App"/> can run in the background.
    /// </summary>
    public interface IBackgroundTask : IIdentifiable<string>
    {
        /// <summary>
        /// Asynchronously executes this <see cref="IBackgroundTask"/>'s action.
        /// </summary>
        Task RunAsync();
    }
}
