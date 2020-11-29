using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// Represents an <see cref="ILifecycleHandler"/> that can provide a task that this <see cref="App"/> can run in the background.
    /// </summary>
    public interface IBackgroundTask : ILifecycleHandler, IIdentifiable<string>
    {
        /// <summary>
        /// The <see cref="BackgroundTaskCapability"/> flags that indicate any capabilities this <see cref="IBackgroundTask"/> will require when running.
        /// </summary>
        BackgroundTaskCapability Capabilities { get; }

        /// <summary>
        /// The <see cref="BackgroundTaskTrigger"/> trigger for executing this <see cref="IBackgroundTask"/>.
        /// </summary>
        BackgroundTaskTrigger Trigger { get; }

        /// <summary>
        /// Asynchronously executes this <see cref="IBackgroundTask"/>'s action.
        /// </summary>
        Task RunAsync();
    }

    /// <summary>
    /// An enum containing the possible capabilities that an <see cref="IBackgroundTask"/> could request while running.
    /// </summary>
    [Flags]
    public enum BackgroundTaskCapability
    {
        /// <summary>
        /// The <see cref="IBackgroundTask"/> will require the internet connectivity stack to remain running (e.g. to use <see cref="System.Net.Http.HttpClient"/>).
        /// </summary>
        Internet = 1
    }
}
