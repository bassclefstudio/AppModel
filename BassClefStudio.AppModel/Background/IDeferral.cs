using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// Represents the deferral of an action such as background launch or suspension in an <see cref="App"/> to allow for the completion of asynchronous tasks in those situations.
    /// </summary>
    public interface IDeferral
    {
        /// <summary>
        /// Starts the <see cref="IDeferral"/>, pausing the system code to allow for asynchronous or long-running app execution.
        /// </summary>
        void StartDeferral();

        /// <summary>
        /// Stops the <see cref="IDeferral"/>, continuing the system code.
        /// </summary>
        void EndDeferral();
    }
}
