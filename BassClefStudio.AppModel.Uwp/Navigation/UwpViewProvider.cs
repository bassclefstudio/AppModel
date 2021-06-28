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
        /// <summary>
        /// A list of all <see cref="ContentControl"/> view layers in the application.
        /// </summary>
        private List<ContentControl> UILayers { get; }
        /// <summary>
        /// Gets the top layer from <see cref="UILayers"/>.
        /// </summary>
        private ContentControl CurrentElement => UILayers[UILayers.Count - 1];

        private INavigationHistory History { get; }
        private IEnumerable<IDispatcher> Dispatchers { get; }
        /// <summary>
        /// Creates a new <see cref="UwpViewProvider"/>.
        /// </summary>
        public UwpViewProvider(IEnumerable<IDispatcher> dispatchers, INavigationHistory history)
        {
            UILayers = new List<ContentControl>();
            Dispatchers = dispatchers;
            History = history;
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

            UILayers.Add(rootFrame);

            //// Ensure the current window is active before setting up back navigation.
            Window.Current.Activate();
        }

        /// <inheritdoc/>
        protected override void SetViewInternal(NavigationRequest request, UIElement view)
        {
            if (request.IsCloseRequest)
            {
                if (UILayers.Count > 1)
                {
                    if(CurrentElement is ContentDialog dialog)
                    {
                        SynchronousTask hideTask = new SynchronousTask(
                        () => Dispatchers.RunOnUIThreadAsync(
                            () => dialog.Hide()));
                        hideTask.RunTask();
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
                    ContentDialog dialog = new ContentDialog()
                    {
                        Title = null,
                        Content = view
                    };
                    UILayers.Add(dialog);
                    dialog.CloseButtonClick += DialogClosed;

                    SynchronousTask showTask = new SynchronousTask(
                        () => Dispatchers.RunOnUIThreadAsync(
                            () => ShowDialogTask(dialog)));
                    showTask.RunTask();
                }
                else if (request.Properties.LayerMode == LayerBehavior.Default)
                {
                    CurrentElement.Content = view;
                }
                else if(request.Properties.LayerMode == LayerBehavior.Shell)
                {
                    if(view is ILayerContainer container)
                    {
                        CurrentElement.Content = container;
                        UILayers.Add(container.Container);
                    }
                    else
                    {
                        throw new ArgumentException("UWP apps require all new navigation layers be created in ILayerContainers.", nameof(view));
                    }
                }
                else
                {
                    throw new ArgumentException($"UWP apps currently do not have support for the given LayerMode {request.Properties.LayerMode}.", nameof(request));
                }
            }
        }

        private void DialogClosed(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //// No need to execute the request, because the platform has executed it for us.
            History.GoBack();
        }

        private async Task<ContentDialogResult> ShowDialogTask(ContentDialog dialog)
        {
            return await dialog.ShowAsync();
        }
    }
}
