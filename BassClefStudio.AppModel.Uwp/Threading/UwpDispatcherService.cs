using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BassClefStudio.AppModel.Threading
{
    /// <summary>
    /// An <see cref="IDispatcherService"/> wrapper for the <see cref="CoreDispatcher"/> to run syncronous/asyncronous code on the UI thread in a UWP application.
    /// </summary>
    public class UwpDispatcherService : IDispatcherService
    {
        /// <summary>
        /// A get/set-able <see cref="bool"/> indicating whether UI thread code needs to be sent through this <see cref="IDispatcherService"/>'s <see cref="CoreDispatcher"/> (i.e. if the app has a foreground UI).
        /// </summary>
        public bool Activated { get; set; } = false;

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Action execute)
        {
            if (Activated)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => execute());
            }
            else
            {
                execute();
            }
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<T> execute)
        {
            if (Activated)
            {
                T output = default(T);
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => output = execute());
                return output;
            }
            else
            {
                return execute();
            }
        }

        public async Task RunOnUIThreadAsync<T>(Func<Task> execute)
        {
            if (Activated)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () => await execute());
            }
            else
            {
                await execute();
            }
        }

        public async Task<T> RunOnUIThreadAsync<T>(Func<Task<T>> execute)
        {
            if (Activated)
            {
                T output = default(T);
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () => output = await execute());
                return output;
            }
            else
            {
                return await execute();
            }
        }
    }
}
