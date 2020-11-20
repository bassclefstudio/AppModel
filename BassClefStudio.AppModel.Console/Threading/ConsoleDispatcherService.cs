using System;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Threading
{
    /// <summary>
    /// An <see cref="IDispatcherService"/> wrapper for console apps that simply runs the given code on the current thread.
    /// </summary>
    public class ConsoleDispatcherService : IDispatcherService
    {
        /// <inheritdoc/>
        public async Task RunOnUIThreadAsync(Action execute)
        {
            execute();
        }

        /// <inheritdoc/>
        public async Task<T> RunOnUIThreadAsync<T>(Func<T> execute)
        {
            return execute();
        }
    }
}
