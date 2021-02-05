﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Represents a service that manages a <see cref="Task"/> that will allow the console application to continue running until the <see cref="App"/> is suspended.
    /// </summary>
    public class LifecycleManager : IInitializationHandler, ISuspendingHandler
    {
        /// <summary>
        /// Represents a <see cref="Task"/> that runs and awaits for the lifetime of the <see cref="App"/>. It is created on <see cref="Initialize(App)"/> and completed on <see cref="Suspend(App)"/>.
        /// </summary>
        public static Task<bool> ApplicationTask { get; private set; }

        /// <summary>
        /// The <see cref="TaskCompletionSource{TResult}"/> managing the <see cref="ApplicationTask"/>.
        /// </summary>
        private static TaskCompletionSource<bool> CompletionSource;

        /// <inheritdoc/>
        public bool Enabled { get; } = true;

        /// <inheritdoc/>
        public bool Initialize(App app)
        {
            CompletionSource = new TaskCompletionSource<bool>();
            ApplicationTask = CompletionSource.Task;
            return true;
        }

        /// <inheritdoc/>
        public bool Suspend(App app)
        {
            CompletionSource.SetResult(true);
            return true;
        }
    }
}