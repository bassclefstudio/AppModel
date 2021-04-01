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
    /// An <see cref="INavigationService"/> built on the WPF's <see cref="ContentControl"/> and <see cref="Window"/> classes.
    /// </summary>
    public class WpfNavigationService : NavigationService<UIElement>, INavigationService
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
                    NavigationStack.Clear();
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="WpfNavigationService"/>.
        /// </summary>
        public WpfNavigationService()
        { }

        /// <inheritdoc/>
        public override void InitializeNavigation()
        {
            var myWindow = new Window();
            Application.Current.MainWindow = myWindow;
            CurrentFrame = myWindow;
            myWindow.Show();
        }

        /// <inheritdoc/>
        protected override bool NavigateInternal(UIElement element)
        {
            if (element is Window window)
            {
                window.Show();
                return false;
            }
            else
            {
                CurrentFrame.Content = element;
                return true;
            }
        }
    }
}