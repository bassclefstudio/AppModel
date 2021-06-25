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
    /// A default implementation of <see cref="INavigationService"/>/<see cref="IBackHandler"/> that extends the <see cref="App"/>'s <see cref="ILifetimeScope"/>, as well as using the <see cref="IViewModel"/>s navigated to as the basis for the <see cref="ICommandRouter"/> command routing.
    /// </summary>
    public class NavigationManager : INavigationService, IBackHandler
    {
        #region Initialization

        /// <summary>
        /// The injected <see cref="IViewProvider"/> for setting view content.
        /// </summary>
        protected IViewProvider ViewProvider { get; }

        /// <summary>
        /// The injected <see cref="INavigationStack"/> for managing history.
        /// </summary>
        protected INavigationStack Stack { get; }

        /// <summary>
        /// The <see cref="ILifetimeScope"/> delegated by the <see cref="App"/> to the <see cref="NavigationManager"/> to resolve view and view-model instances.
        /// </summary>
        protected ILifetimeScope LifetimeScope { get; }

        /// <summary>
        /// The injected <see cref="ICommandRouter"/> for setting active <see cref="IViewModel"/>s.
        /// </summary>
        protected ICommandRouter CommandRouter { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationManager"/> from the provided services.
        /// </summary>
        public NavigationManager(IViewProvider viewProvider, INavigationStack stack, ILifetimeScope scope, ICommandRouter router)
        {
            ViewProvider = viewProvider;
            Stack = stack;
            LifetimeScope = scope;
            CommandRouter = router;
        }

        #endregion
        #region Methods

        /// <inheritdoc/>
        public void Navigate(NavigationRequest request)
        {
            IViewModel viewModel = null;
            IView view = null;
            Type viewType = null;

            if (request.ContainsInstance)
            {
                viewModel = request.ViewModelInstance;
                viewType = typeof(IView<>).MakeGenericType(request.ViewModelInstance.GetType());
                view = (IView)LifetimeScope.Resolve(viewType);
            }
            else
            {
                viewModel = (IViewModel)LifetimeScope.Resolve(request.ViewModelType);
                viewType = typeof(IView<>).MakeGenericType(request.ViewModelType);
                view = (IView)LifetimeScope.Resolve(viewType);
            }

            viewType.GetProperty("ViewModel").SetValue(view, viewModel);
            ViewProvider.SetView(view, request.Mode);

            //// Sets ICommandRouter to manage active view-model.
            CommandRouter.ActiveCommandHandlers.Clear();
            CommandRouter.ActiveCommandHandlers.Add(viewModel);

            //// Initializes the view-model and view.
            view.Initialize();
            SynchronousTask initViewModelTask =
                new SynchronousTask(() => viewModel.InitializeAsync(request.Parameter));
            initViewModelTask.RunTask();
            Stack.AddRequest(request);
        }

        /// <inheritdoc/>
        public bool GoBack()
        {
            if(Stack.CanGoBack)
            {
                this.GoBack(Stack);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// A generic implementation of <see cref="INavigationStack"/> that simply stores and traverses a linear history.
    /// </summary>
    public class NavigationStack : Observable, INavigationStack
    {
        #region Properties

        private SourceStream<NavigationRequest> requestStream;
        /// <inheritdoc/>
        public IStream<NavigationRequest> RequestStream => requestStream;

        private bool canGoBack;
        /// <inheritdoc/>
        public bool CanGoBack { get => canGoBack; private set => Set(ref canGoBack, value); }

        private bool canGoForward;
        /// <inheritdoc/>
        public bool CanGoForward { get => canGoForward; private set => Set(ref canGoForward, value); }

        /// <summary>
        /// The indexed position in <see cref="Requests"/> of the current history.
        /// </summary>
        protected int HistoryPosition { get; set; }

        /// <summary>
        /// A collection of all <see cref="NavigationRequest"/> previous requests.
        /// </summary>
        protected List<NavigationRequest> Requests { get; }

        #endregion
        #region Initialization
        
        /// <summary>
        /// Creates a new <see cref="NavigationStack"/>.
        /// </summary>
        public NavigationStack()
        {
            Requests = new List<NavigationRequest>();
            requestStream = new SourceStream<NavigationRequest>();
            HistoryPosition = -1;
            SetCanGo();
        }

        #endregion
        #region Methods

        /// <inheritdoc/>
        public void AddRequest(NavigationRequest request)
        {
            if (!request.Mode.IgnoreHistory && HistoryPosition >= 0 && request != Requests[HistoryPosition])
            {
                if (HistoryPosition + 1 != Requests.Count)
                {
                    Requests.RemoveRange(HistoryPosition + 1, Requests.Count - (HistoryPosition + 1));
                }

                Requests.Add(request);
                HistoryPosition++;
                SetCanGo();
                requestStream.EmitValue(request);
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Requests.Clear();
            HistoryPosition = -1;
            SetCanGo();
        }

        /// <inheritdoc/>
        public NavigationRequest GoBack()
        {
            if (CanGoBack)
            {
                HistoryPosition--;
                return Requests[HistoryPosition];
            }
            else
            {
                throw new InvalidOperationException("Cannot currently go back in the navigation stack.");
            }
        }

        /// <inheritdoc/>
        public NavigationRequest GoForward()
        {
            if (CanGoForward)
            {
                HistoryPosition++;
                return Requests[HistoryPosition];
            }
            else
            {
                throw new InvalidOperationException("Cannot currently go back in the navigation stack.");
            }
        }
        
        /// <summary>
        /// Sets the <see cref="CanGoBack"/> and <see cref="CanGoForward"/> properties accordingly.
        /// </summary>
        protected void SetCanGo()
        {
            CanGoBack = HistoryPosition > 0;
            CanGoForward = HistoryPosition + 1 < Requests.Count;
        }

        #endregion
    }
}
