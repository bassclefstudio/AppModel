using BassClefStudio.AppModel.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.AppModel.Navigation
{
    public class UwpNavigationService : INavigationService
    {
        public Frame CurrentFrame { get; set; }
        
        /// <inheritdoc/>
        public void InitializeNavigation()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            //// Do not repeat app initialization when the Window already has content,
            //// just ensure that the window is active
            if (rootFrame == null)
            {
                //// Create a Frame to act as the navigation context.
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            CurrentFrame = rootFrame;

            //// Ensure the current window is active before setting up back navigation.
            Window.Current.Activate();
        }

        /// <inheritdoc/>
        public event EventHandler Navigated;

        /// <inheritdoc/>
        public void Navigate(IView view, object parameter = null)
        {
            if(!(view is UIElement))
            {
                Debug.Write($"UWP Navigation usually expects that the resolved IViews be UIElements. View type: {view?.GetType().Name}");
            }

            CurrentFrame.Content = view;
            Navigated?.Invoke(this, new EventArgs());
        }
    }
}
