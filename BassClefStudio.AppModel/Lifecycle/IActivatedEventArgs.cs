using BassClefStudio.AppModel.Background;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Information about the specific way in which an <see cref="App"/> was started.
    /// </summary>
    public interface IActivatedEventArgs
    {
    }

    /// <summary>
    /// The default <see cref="IActivatedEventArgs"/>, provided when the user normally started an <see cref="App"/>.
    /// </summary>
    public class LaunchActivatedEventArgs : IActivatedEventArgs
    {
        /// <summary>
        /// Optional <see cref="string"/> arguments passed to the <see cref="App"/> when it was launched.
        /// </summary>
        public string[] Args { get; }

        /// <summary>
        /// Creates a new <see cref="LaunchActivatedEventArgs"/>.
        /// </summary>
        /// <param name="args">Optional <see cref="string"/> arguments passed to the <see cref="App"/> when it was launched.</param>
        public LaunchActivatedEventArgs(params string[] args)
        {
            Args = args;
        }
    }

    /// <summary>
    /// The base <see cref="IActivatedEventArgs"/> for <see cref="App"/> activation that occurs in the background and does not trigger UI.
    /// </summary>
    public abstract class BackgroundActivatedEventArgs : IActivatedEventArgs
    {
        /// <summary>
        /// The identifiable name of the background task that should be executed.
        /// </summary>
        public string TaskName { get; }

        /// <summary>
        /// Creates or retreieves the <see cref="IDeferral"/> that can be used for managing this background task.
        /// </summary>
        public abstract IDeferral GetDeferral();

        /// <summary>
        /// Creates a new <see cref="BackgroundActivatedEventArgs"/>.
        /// </summary>
        /// <param name="taskName">The identifiable name of the background task that should be executed.</param>
        public BackgroundActivatedEventArgs(string taskName)
        {
            TaskName = taskName;
        }
    }
}
