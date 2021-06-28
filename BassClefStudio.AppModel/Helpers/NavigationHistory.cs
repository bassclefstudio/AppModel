using Autofac;
using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// A default implementation of the <see cref="INavigationHistory"/> interface.
    /// </summary>
    public class NavigationHistory : INavigationHistory
    {
        private SourceStream<NavigationRequest> requestStream;
        /// <inheritdoc/>
        public IStream<NavigationRequest> RequestStream { get; }

        private List<INavigationLayer> layers;
        /// <inheritdoc/>
        public IEnumerable<INavigationLayer> Layers => layers.AsEnumerable();

        /// <inheritdoc/>
        public INavigationLayer CurrentLayer => layers[layers.Count - 1];

        /// <summary>
        /// An injected function for creating new <see cref="INavigationLayer"/>s.
        /// </summary>
        private Func<INavigationLayer> CreateLayer { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationHistory"/> history.
        /// </summary>
        public NavigationHistory(Func<INavigationLayer> createLayer)
        {
            requestStream = new SourceStream<NavigationRequest>();
            RequestStream = requestStream.UniqueEq();

            layers = new List<INavigationLayer>();
            CreateLayer = createLayer;

            Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            layers.Clear();
            AddLayer(false);
        }

        private void AddLayer(bool isReturnable)
        {
            layers.Add(CreateLayer());
            CurrentLayer.IsReturnable = isReturnable;
        }

        /// <inheritdoc/>
        public bool CanGoBack => CurrentLayer.CanGoBack || (layers.Count > 0 && CurrentLayer.IsReturnable);

        /// <inheritdoc/>
        public NavigationRequest GoBack()
        {
            if(CurrentLayer.CanGoBack)
            {
                var req = CurrentLayer.GoBack();
                requestStream.EmitValue(req);
                return req;
            }
            else if(layers.Count > 1 && CurrentLayer.IsReturnable)
            {
                layers.RemoveAt(layers.Count - 1);
                var req = NavigationRequest.Close;
                requestStream.EmitValue(req);
                return req;
            }
            else
            {
                throw new InvalidOperationException("The current state of the NavigationHistory does not support back navigation.");
            }
        }

        /// <inheritdoc/>
        public bool CanGoForward => CurrentLayer.CanGoForward;

        /// <inheritdoc/>
        public NavigationRequest GoForward()
        {
            if (CurrentLayer.CanGoForward)
            {
                return CurrentLayer.GoForward();
            }
            else
            {
                throw new InvalidOperationException("The current state of the NavigationHistory does not support forward navigation.");
            }
        }

        /// <inheritdoc/>
        public void HandleNavigation(NavigationRequest request, IViewModel viewModel)
        {
            if (request.IsCloseRequest)
            {
                //// Do nothing - this is a close request, which is not stored in history.
            }
            if(request.Properties.LayerMode == LayerBehavior.Default)
            {
                CurrentLayer.HandleNavigation(request, viewModel);
            }
            else if (request.Properties.LayerMode == LayerBehavior.Modal)
            {
                AddLayer(true);
                CurrentLayer.HandleNavigation(request, viewModel);
            }
            else if(request.Properties.LayerMode == LayerBehavior.Shell)
            {
                CurrentLayer.HandleNavigation(request, viewModel);
                AddLayer(false);
            }
            else
            {
                throw new ArgumentException($"Request contained unknown LayerBehavior {request.Properties.LayerMode}", nameof(request));
            }

            requestStream.EmitValue(request);
        }
    }

    /// <summary>
    /// A default implementation of the <see cref="INavigationLayer"/> interface.
    /// </summary>
    public class NavigationLayer : INavigationLayer
    {
        /// <inheritdoc/>
        public IViewModel CurrentViewModel { get; private set; }

        /// <inheritdoc/>
        public bool IsReturnable { get; set; }
        
        /// <summary>
        /// The current index in navigation history.
        /// </summary>
        private int CurrentIndex { get; set; }

        private List<NavigationRequest> History { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationLayer"/> layer.
        /// </summary>
        public NavigationLayer()
        {
            History = new List<NavigationRequest>();
            Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            History.Clear();
            CurrentIndex = -1;
        }

        /// <summary>
        /// Adds an item to <see cref="History"/>, keeping the <see cref="CurrentIndex"/> head intact.
        /// </summary>
        /// <param name="request">The <see cref="NavigationRequest"/> to add.</param>
        private void AddHistory(NavigationRequest request)
        {
            if(History.Count > 0 && CurrentIndex < History.Count - 1)
            {
                History.RemoveRange(CurrentIndex + 1, History.Count - 1);
            }

            History.Add(request);
            CurrentIndex++;
        }

        /// <inheritdoc/>
        public bool CanGoBack => CurrentIndex > 0;

        /// <inheritdoc/>
        public NavigationRequest GoBack()
        {
            if(CanGoBack)
            {
                CurrentIndex--;
                return History[CurrentIndex];
            }
            else
            {
                throw new InvalidOperationException("The current state of the NavigationLayer does not support back navigation.");
            }
        }

        /// <inheritdoc/>
        public bool CanGoForward => CurrentIndex < (History.Count - 1);

        /// <inheritdoc/>
        public NavigationRequest GoForward()
        {
            if (CanGoForward)
            {
                CurrentIndex++;
                return History[CurrentIndex];
            }
            else
            {
                throw new InvalidOperationException("The current state of the NavigationLayer does not support forward navigation.");
            }
        }

        /// <inheritdoc/>
        public void HandleNavigation(NavigationRequest request, IViewModel viewModel)
        {
            CurrentViewModel = viewModel;

            if (request.Properties.HistoryMode == HistoryBehavior.Default)
            {
                AddHistory(request);
            }
            else if(request.Properties.HistoryMode == HistoryBehavior.Block)
            {
                Clear();
            }
            else if(request.Properties.HistoryMode == HistoryBehavior.Skip)
            {
                //// Nothing happens, because history recording is skipped.
            }
            else
            {
                throw new ArgumentException($"Request contained unknown HistoryBehavior {request.Properties.HistoryMode}", nameof(request));
            }
        }
    }
}
