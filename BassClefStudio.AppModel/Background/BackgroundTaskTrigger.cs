using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// Represents a trigger for an <see cref="IBackgroundTask"/>.
    /// </summary>
    public abstract class BackgroundTaskTrigger
    {
    }

    /// <summary>
    /// A <see cref="BackgroundTaskTrigger"/> that triggers after a certain length of time.
    /// </summary>
    public class TimeBackgroundTaskTrigger : BackgroundTaskTrigger
    {
        /// <summary>
        /// A <see cref="TimeSpan"/> indicating the desired amount of time before the <see cref="IBackgroundTask"/> is triggered.
        /// </summary>
        public TimeSpan Time { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="IBackgroundTask"/> should continue to be fired after every <see cref="Time"/> has elapsed.
        /// </summary>
        public bool Continuous { get; }

        /// <summary>
        /// Creates a new <see cref="TimeBackgroundTaskTrigger"/>.
        /// </summary>
        /// <param name="timeSpan">A <see cref="TimeSpan"/> indicating the desired amount of time before the <see cref="IBackgroundTask"/> is triggered.</param>
        /// <param name="continuous">A <see cref="bool"/> indicating whether the <see cref="IBackgroundTask"/> should continue to be fired after every <see cref="Time"/> has elapsed.</param>
        public TimeBackgroundTaskTrigger(TimeSpan timeSpan, bool continuous)
        {
            Time = timeSpan;
            Continuous = continuous;
        }
    }
}
