using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BassClefStudio.AppModel.Threading
{
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
