using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// The UWP implementation of <see cref="IBackgroundService"/>, which manages background tasks in UWP using the system broker.
    /// </summary>
    public class UwpBackgroundService : IBackgroundService, IInitializationHandler
    {
        /// <summary>
        /// The collection of UWP <see cref="IBackgroundTaskRegistration"/>s that define the currently registered background tasks.
        /// </summary>
        internal List<IBackgroundTaskRegistration> Registrations { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<string> CurrentlyRegistered => Registrations.Select(r => r.Name);

        /// <inheritdoc/>
        public bool Enabled { get; } = true;

        /// <summary>
        /// Creates a new <see cref="UwpBackgroundService"/>.
        /// </summary>
        public UwpBackgroundService()
        { }

        /// <inheritdoc/>
        public bool Initialize()
        {
            Registrations = new List<IBackgroundTaskRegistration>(BackgroundTaskRegistration.AllTasks.Values);
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> RegisterAsync(IBackgroundTask task)
        {
            var backgroundStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy
               || backgroundStatus == BackgroundAccessStatus.AlwaysAllowed)
            {
                var builder = new BackgroundTaskBuilder();
                builder.IsNetworkRequested = task.Capabilities.HasFlag(BackgroundTaskCapability.Internet);
                builder.Name = task.Id;
                builder.SetTrigger(task.Trigger.GetTrigger());
                Registrations.Add(builder.Register());
                Debug.WriteLine($"Background task {task.Id} registered.");
                return true;
            }
            else
            {
                Debug.WriteLine($"Background activation of {task.Id} has been disabled by permission or power saving status.");
                return false;
            }
        }

        /// <inheritdoc/>
        public void UnRegister(string taskName)
        {
            var task = Registrations.FirstOrDefault(r => r.Name == taskName);
            if(task != null)
            {
                task.Unregister(false);
                Registrations.Remove(task);
            }
        }
    }

    /// <summary>
    /// Provides UWP platform extensions for <see cref="IBackgroundTask"/> and related types.
    /// </summary>
    internal static class BackgroundTaskExtensions
    {
        public static IBackgroundTrigger GetTrigger(this BackgroundTaskTrigger trigger)
        {
            if(trigger is TimeBackgroundTaskTrigger timeTrigger)
            {
                return new TimeTrigger((uint)timeTrigger.Time.TotalMinutes, !timeTrigger.Continuous);
            }
            else
            {
                throw new ArgumentException($"BackgroundTaskTrigger of unknown type {trigger.GetType().Name}", nameof(trigger));
            }
        }
    }
}
