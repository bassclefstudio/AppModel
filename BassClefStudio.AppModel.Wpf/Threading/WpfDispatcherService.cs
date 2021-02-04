using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BassClefStudio.AppModel.Threading
{
    /// <summary>
    /// An <see cref="IDispatcherService"/> that uses the <see cref="Application.Current"/>'s dispatcher to execute code on the UI thread.
    /// </summary>
    public class WpfDispatcherService : IDispatcherService
    {
        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Action execute)
        {
            await Application.Current.Dispatcher.InvokeAsync(() => execute());
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<T> execute)
        {
            return await Application.Current.Dispatcher.InvokeAsync(() => execute());
        }

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync<T>(Func<Task> execute)
        {
            //// Uh-huh, this doesn't seem right.
            await Application.Current.Dispatcher.InvokeAsync(async () => await execute());
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<Task<T>> execute)
        {
            //// Uh-huh, this doesn't seem right.
            return await await Application.Current.Dispatcher.InvokeAsync(() => execute());
        }
    }
}
