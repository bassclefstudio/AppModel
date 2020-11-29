using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;

namespace BassClefStudio.AppModel.Background
{
    /// <summary>
    /// A UWP wrapper around <see cref="Windows.Foundation.Deferral"/> which implements the <see cref="IDeferral"/> interface.
    /// </summary>
    public class UwpDeferral : IDeferral, IDisposable
    {
        /// <summary>
        /// A <see cref="Func{TResult}"/> which creates the attached <see cref="Windows.Foundation.Deferral"/>.
        /// </summary>
        internal Func<Deferral> GetDeferral { get; }

        /// <summary>
        /// The attached <see cref="Windows.Foundation.Deferral"/>.
        /// </summary>
        internal Deferral Deferral { get; private set; }

        /// <summary>
        /// Creates a new <see cref="UwpDeferral"/>.
        /// </summary>
        /// <param name="deferral">A <see cref="Func{TResult}"/> which creates the attached <see cref="Windows.Foundation.Deferral"/>.</param>
        public UwpDeferral(Func<Deferral> getDeferral)
        {
            GetDeferral = getDeferral;
        }

        /// <inheritdoc/>
        public void StartDeferral()
        {
            if (Deferral == null)
            {
                Deferral = GetDeferral();
            }
        }

        /// <inheritdoc/>
        public void EndDeferral()
        {
            if (Deferral != null)
            {
                Deferral.Complete();
                Dispose();
                Deferral = null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Deferral?.Dispose();
        }
    }

    /// <summary>
    /// A UWP wrapper around <see cref="UwpBackgroundDeferral"/> which implements the <see cref="IDeferral"/> interface.
    /// </summary>
    public class UwpBackgroundDeferral : IDeferral
    {
        /// <summary>
        /// A <see cref="Func{TResult}"/> which creates the attached <see cref="BackgroundTaskDeferral"/>.
        /// </summary>
        internal Func<BackgroundTaskDeferral> GetDeferral { get; }

        /// <summary>
        /// The attached <see cref="BackgroundTaskDeferral"/>.
        /// </summary>
        internal BackgroundTaskDeferral Deferral { get; private set; }

        /// <summary>
        /// Creates a new <see cref="UwpBackgroundDeferral"/>.
        /// </summary>
        /// <param name="deferral">A <see cref="Func{TResult}"/> which creates the attached <see cref="BackgroundTaskDeferral"/>.</param>
        public UwpBackgroundDeferral(Func<BackgroundTaskDeferral> getDeferral)
        {
            GetDeferral = getDeferral;
        }

        /// <inheritdoc/>
        public void StartDeferral()
        {
            if (Deferral == null)
            {
                Deferral = GetDeferral();
            }
        }

        /// <inheritdoc/>
        public void EndDeferral()
        {
            if (Deferral != null)
            {
                Deferral.Complete();
                Deferral = null;
            }
        }
    }
}
