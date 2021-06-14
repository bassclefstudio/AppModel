using BassClefStudio.AppModel.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// An <see cref="IViewProvider"/> built on the WPF's <see cref="ContentControl"/> and <see cref="Window"/> classes.
    /// </summary>
    public class WpfViewProvider : ViewProvider<UIElement>, IViewProvider
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
                if (currentFrame != value)
                {
                    currentFrame = value;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="WpfViewProvider"/>.
        /// </summary>
        public WpfViewProvider()
        { }

        /// <inheritdoc/>
        public override void StartUI()
        {
            var myWindow = new Window();
            Application.Current.MainWindow = myWindow;
            CurrentFrame = myWindow;
            myWindow.Show();
        }

        private Window currentDialog = null;
        private bool dialogDisplaying = false;

        /// <inheritdoc/>
        protected override void SetViewInternal(UIElement element, NavigationMode mode)
        {
            if(currentDialog != null)
            {
                dialogDisplaying = false;
                currentDialog.Close();
                currentDialog = null;
            }

            if (mode.OverlayMode == NavigationOverlay.Override)
            {
                Application.Current.MainWindow.Content = element;
            }
            else if (mode.OverlayMode == NavigationOverlay.Page)
            {
                CurrentFrame.Content = element;
            }
            else if (mode.OverlayMode == NavigationOverlay.Modal)
            {
                var window = new Window()
                {
                    Content = element,
                    WindowStyle = WindowStyle.None,
                    ShowInTaskbar = false,
                    MinHeight = 200,
                    MinWidth = 300,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.NoResize,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                dialogDisplaying = true;
                currentDialog = window;
                window.Closing += DialogClosing;
                window.ShowDialog();
            }
            else if (mode.OverlayMode == NavigationOverlay.Window)
            {
                var window = new Window()
                {
                    Content = element,
                };

                window.Show();
            }
            else
            {
                throw new ArgumentException($"UWP apps currently do not have support for the given OverlayMode {mode.OverlayMode}.");
            }
        }

        private void DialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = dialogDisplaying;
        }
    }
}