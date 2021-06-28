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
        /// <summary>
        /// A list of all <see cref="ContentControl"/> view layers in the application.
        /// </summary>
        private List<ContentControl> UILayers { get; }
        /// <summary>
        /// Gets the top layer from <see cref="UILayers"/>.
        /// </summary>
        private ContentControl CurrentElement => UILayers[UILayers.Count - 1];

        private INavigationHistory History { get; }
        /// <summary>
        /// Creates a new <see cref="WpfViewProvider"/>.
        /// </summary>
        public WpfViewProvider(INavigationHistory history)
        {
            History = history;
        }

        /// <inheritdoc/>
        public override void StartUI()
        {
            var myWindow = new Window();
            Application.Current.MainWindow = myWindow;
            UILayers.Add(myWindow);
            myWindow.Show();
        }

        /// <inheritdoc/>
        protected override void SetViewInternal(NavigationRequest request, UIElement view)
        {
            if (request.IsCloseRequest)
            {
                if (UILayers.Count > 1)
                {
                    if (CurrentElement is Window dialog)
                    {
                        dialog.Close();
                    }

                    UILayers.RemoveAt(UILayers.Count - 1);
                }
                else
                {
                    throw new InvalidOperationException("Cannot remove root content layer in close operation.");
                }
            }
            else
            {
                if (request.Properties.LayerMode == LayerBehavior.Modal)
                {
                    var dialog = new Window()
                    {
                        Content = view,
                        WindowStyle = WindowStyle.None,
                        ShowInTaskbar = false,
                        MinHeight = 200,
                        MinWidth = 300,
                        SizeToContent = SizeToContent.WidthAndHeight,
                        ResizeMode = ResizeMode.NoResize,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    UILayers.Add(dialog);
                    dialog.Closing += DialogClosing;
                    dialog.ShowDialog();
                }
                else if (request.Properties.LayerMode == LayerBehavior.Default)
                {
                    CurrentElement.Content = view;
                }
                else if (request.Properties.LayerMode == LayerBehavior.Shell)
                {
                    if (view is ILayerContainer container)
                    {
                        CurrentElement.Content = container;
                        UILayers.Add(container.Container);
                    }
                    else
                    {
                        throw new ArgumentException("WPF apps require all new navigation layers be created in ILayerContainers.", nameof(view));
                    }
                }
                else
                {
                    throw new ArgumentException($"WPF apps currently do not have support for the given LayerMode {request.Properties.LayerMode}.", nameof(request));
                }
            }
        }

        private void DialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //// No need to execute the request, because the platform has executed it for us.
            History.GoBack();
        }
    }
}