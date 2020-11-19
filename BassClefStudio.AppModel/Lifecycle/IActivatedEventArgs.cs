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

    //// C# 9 record?
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
}
