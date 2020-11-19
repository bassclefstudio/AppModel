using System;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace BassClefStudio.AppModel.Threading.Uwp
{
    /// <summary>
    /// An <see cref="IDispatcherService"/> wrapper for the <see cref="CoreDispatcher"/> to run syncronous/asyncronous code on the UI thread in a UWP application.
    /// </summary>
    public class UwpDispatcherService : IDispatcherService
    {
        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Action execute)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => execute());
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<T> execute)
        {
            T output = default(T);
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => output = execute());
            return output;
        }
    }
}
