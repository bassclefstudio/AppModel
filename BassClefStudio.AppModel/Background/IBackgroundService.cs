using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// Represents a platform-provided service for registering and unregistering <see cref="IBackgroundTask"/>s.
    /// </summary>
    public interface IBackgroundService
    {
        /// <summary>
        /// A collection of <see cref="string"/> IDs for all of the registered background tasks in the system.
        /// </summary>
        IEnumerable<string> CurrentlyRegistered { get; }

        /// <summary>
        /// Registers an <see cref="IBackgroundTask"/> to the <see cref="IBackgroundService"/>, allowing it to be triggered in the background by the system at a later time.
        /// </summary>
        /// <param name="task">The task to register.</param>
        /// <returns>A <see cref="bool"/> indicating whether the operation succeeded.</returns>
        Task<bool> RegisterAsync(IBackgroundTask task);

        /// <summary>
        /// Removes the registration of the <see cref="IBackgroundTask"/> with the given name from this <see cref="IBackgroundService"/>.
        /// </summary>
        /// <param name="taskName">The <see cref="IBackgroundTask"/>'s <see cref="string"/> ID, as found in the <see cref="CurrentlyRegistered"/> collection.</param>
        void UnRegister(string taskName);
    }

    /// <summary>
    /// Contains extension methods for the <see cref="IBackgroundService"/> interface.
    /// </summary>
    public static class BackgroundServiceExtensions
    {
        /// <summary>
        /// Syncs the currently registered <see cref="IBackgroundTask"/>s on the given <see cref="IBackgroundService"/> to match a collection of <see cref="IBackgroundTask"/>s.
        /// </summary>
        /// <param name="service">The <see cref="IBackgroundService"/> used for registering/unregistering background tasks.</param>
        /// <param name="tasks">The desired collection of <see cref="IBackgroundTask"/>s to register.</param>
        public static async Task RegisterCollectionAsync(this IBackgroundService service, IEnumerable<IBackgroundTask> tasks)
        {
            var toRemove = service.CurrentlyRegistered.Where(r => !tasks.Any(t => t.Id == r)).ToArray();
            var toAdd = tasks.Where(t => !service.CurrentlyRegistered.Any(r => r == t.Id)).ToArray();
            foreach (var name in toRemove)
            {
                service.UnRegister(name);
            }

            foreach (var task in toAdd)
            {
                await service.RegisterAsync(task);
            }
        }
    }
}
