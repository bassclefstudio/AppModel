using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Threading
{
    /// <summary>
    /// A service that can send tasks to be run on different threads, including the UI thread (for non-UI projects or single-threaded apps, this service will not change code execution).
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Runs the given code on the UI thread.
        /// </summary>
        /// <param name="execute">An <see cref="Action"/> to execute on the UI thread.</param>
        Task RunOnUIThreadAsync(Action execute);

        /// <summary>
        /// Runs the given code on the UI thread.
        /// </summary>
        /// <param name="execute">A <see cref="Func{TResult}"/> to execute on the UI thread.</param>
        /// <returns>The <typeparamref name="T"/> result of the <paramref name="execute"/> function.</returns>
        Task<T> RunOnUIThreadAsync<T>(Func<T> execute);
    }
}
