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
    public class WpfNavigationService : INavigationService
    {
        /// <summary>
        /// The current frame for navigation content.
        /// </summary>
        public ContentControl CurrentFrame { get; set; }

        internal IDispatcherService DispatcherService { get; }
        /// <summary>
        /// Creates a new <see cref="WpfNavigationService"/>.
        /// </summary>
        /// <param name="dispatcherService">The <see cref="IDispatcherService"/> for running UI code on the correct thread.</param>
        public WpfNavigationService(IDispatcherService dispatcherService)
        {
            DispatcherService = dispatcherService;
        }

        /// <inheritdoc/>
        public void InitializeNavigation()
        {
            var myWindow = new Window();
            Application.Current.MainWindow = myWindow;
            CurrentFrame = myWindow;
            myWindow.Show();
        }

        /// <inheritdoc/>
        public void Navigate(IView view, object parameter = null)
        {
            if (!(view is UIElement))
            {
                Debug.Write($"WPF Navigation usually expects that the resolved IViews be UIElements. View type: {view?.GetType().Name}");
            }

            if (view is Window window)
            {
                window.Show();
            }
            else
            {
                CurrentFrame.Content = view;
            }
        }
    }
}