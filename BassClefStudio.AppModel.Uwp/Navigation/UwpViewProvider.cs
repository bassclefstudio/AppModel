using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// An <see cref="IViewProvider"/> built on the UWP's <see cref="ContentControl"/> and <see cref="Window"/> classes.
    /// </summary>
    public class UwpViewProvider : ViewProvider<UIElement>, IViewProvider
    {
        private ContentControl currentFrame;
        /// <summary>
        /// The current frame for navigation content.
        /// </summary>
        public ContentControl CurrentFrame
        { 
            get => currentFrame;
            set
            {
                if(currentFrame != value)
                {
                    currentFrame = value;
                }
            }
        }

        private IEnumerable<IDispatcher> Dispatchers { get; }
        /// <summary>
        /// Creates a new <see cref="UwpViewProvider"/>.
        /// </summary>
        public UwpViewProvider(IEnumerable<IDispatcher> dispatchers)
        {
            Dispatchers = dispatchers;
        }

        /// <inheritdoc/>
        public override void StartUI()
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
        protected override void SetViewInternal(UIElement element)
        {
            if (element is ContentDialog dialog)
            {
                SynchronousTask showTask = new SynchronousTask(
                    () => Dispatchers.RunOnUIThreadAsync(
                        () => ShowDialogTask(dialog)));
                showTask.RunTask();
            }
            else
            {
                CurrentFrame.Content = element;
            }
        }

        private async Task<ContentDialogResult> ShowDialogTask(ContentDialog dialog)
        {
            return await dialog.ShowAsync();
        }
    }
}
