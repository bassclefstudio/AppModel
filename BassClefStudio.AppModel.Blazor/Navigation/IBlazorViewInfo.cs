using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// A service that provides information about the currently navigated <see cref="BlazorView"/>.
    /// </summary>
    public interface IBlazorViewInfo
    {
        /// <summary>
        /// The current <see cref="BlazorView"/> view.
        /// </summary>
        BlazorView CurrentView { get; set; }

        /// <summary>
        /// An event fired whenever the <see cref="CurrentView"/> is changed.
        /// </summary>
        event EventHandler CurrentViewChanged;
    }

    /// <summary>
    /// A default implementation of <see cref="IBlazorViewInfo"/>.
    /// </summary>
    public class BlazorViewInfo : IBlazorViewInfo
    {
        private BlazorView currentView;
        /// <inheritdoc/>
        public BlazorView CurrentView
        { 
            get => currentView; 
            set
            {
                if(currentView != value)
                {
                    currentView = value;
                    CurrentViewChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <inheritdoc/>
        public event EventHandler CurrentViewChanged;
    }
}
