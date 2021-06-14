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

        /// <inheritdoc/>
        protected override void SetViewInternal(UIElement element)
        {
            if (element is Window window)
            {
                window.Show();
            }
            else
            {
                CurrentFrame.Content = element;
            }
        }
    }
}