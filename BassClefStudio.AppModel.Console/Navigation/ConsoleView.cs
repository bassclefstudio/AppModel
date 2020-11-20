using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    public abstract class ConsoleView : IView
    {
        /// <summary>
        /// An event fired when the content of this <see cref="ConsoleView"/> has been updated.
        /// </summary>
        public event EventHandler Updated;

        /// <summary>
        /// Requests an update to the displayed content for this <see cref="ConsoleView"/>, firing the <see cref="Updated"/> event.
        /// </summary>
        protected void RequestUpdate()
        {
            Updated?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Writes the current content of the <see cref="ConsoleView"/> to the <see cref="Console"/>.
        /// </summary>
        public abstract Task DrawContentAsync();
    }
}
