using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Threading
{
    /// <summary>
    /// A basic <see cref="IDispatcher"/> which simply runs the given code.
    /// </summary>
    public class BaseDispatcher : IDispatcher
    {
        /// <inheritdoc/>
        public DispatcherType DispatcherType { get; }

        /// <summary>
        /// Creates a new <see cref="BaseDispatcher"/> with the specified flags.
        /// </summary>
        /// <param name="dispatcherType">A set of <see cref="DispatcherType"/> flags indicating whether this <see cref="IDispatcher"/> manages special kinds of threads, which can (and should) be utilized in scenarios such as updating UI elements from code.</param>
        public BaseDispatcher(DispatcherType dispatcherType = DispatcherType.None)
        {
            DispatcherType = dispatcherType;
        }

        /// <inheritdoc/>
        public async Task RunAsync(Action execute)
        {
            execute();
        }

        /// <inheritdoc/>
        public async Task<T> RunAsync<T>(Func<T> execute)
        {
            return execute();
        }

        /// <inheritdoc/>
        public async Task RunAsync(Func<Task> execute)
        {
            await execute();
        }

        /// <inheritdoc/>
        public async Task<T> RunAsync<T>(Func<Task<T>> execute)
        {
            return await execute();
        }
    }
}
