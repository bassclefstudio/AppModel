using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
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
    /// <summary>
    /// An <see cref="INavigationService"/> built on the UWP's <see cref="ContentControl"/> and <see cref="Window"/> classes.
    /// </summary>
    public class UwpNavigationService : INavigationService
    {
        /// <summary>
        /// The current frame for navigation content.
        /// </summary>
        public ContentControl CurrentFrame { get; set; }
        
        internal IDispatcherService DispatcherService { get; }
        public UwpNavigationService(IDispatcherService dispatcherService)
        {
            DispatcherService = dispatcherService;
        }

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
        public void Navigate(IView view, object parameter = null)
        {
            if(!(view is UIElement))
            {
                Debug.Write($"UWP Navigation usually expects that the resolved IViews be UIElements. View type: {view?.GetType().Name}");
            }

            if (view is ContentDialog dialog)
            {
                SynchronousTask showTask = new SynchronousTask(
                    () => DispatcherService.RunOnUIThreadAsync(
                        () => ShowDialogTask(dialog)));
                showTask.RunTask();
            }
            else
            {
                CurrentFrame.Content = view;
            }
        }

        private async Task<ContentDialogResult> ShowDialogTask(ContentDialog dialog)
        {
            return await dialog.ShowAsync();
        }

    }
}
