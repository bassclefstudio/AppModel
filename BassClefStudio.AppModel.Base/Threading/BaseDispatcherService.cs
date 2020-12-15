using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Threading
{
    /// <summary>
    /// A basic <see cref="IDispatcherService"/> which simply runs the given code (does not look for specific UI thread).
    /// </summary>
    public class BaseDispatcherService : IDispatcherService
    {
        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Action execute)
        {
            await Task.Run(execute);
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<T> execute)
        {
            return await Task.Run(execute);
        }

        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync<T>(Func<Task> execute)
        {
            await execute();
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<Task<T>> execute)
        {
            return await execute();
        }
    }
}
