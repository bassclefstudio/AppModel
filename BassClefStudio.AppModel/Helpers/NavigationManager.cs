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
    public class NavigationService : INavigationService, IBackHandler
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
        /// The injected <see cref="INavigationActiveHandler"/> for managing active view-models.
        /// </summary>
        protected INavigationActiveHandler ActiveHandler { get; }

        /// <summary>
        /// The <see cref="ILifetimeScope"/> delegated by the <see cref="App"/> to the <see cref="NavigationService"/> to resolve view and view-model instances.
        /// </summary>
        protected ILifetimeScope LifetimeScope { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationService"/> from the provided services.
        /// </summary>
        public NavigationService(IViewProvider viewProvider, INavigationStack stack, INavigationActiveHandler activeHandler, ILifetimeScope scope)
        {
            ViewProvider = viewProvider;
            Stack = stack;
            ActiveHandler = activeHandler;
            LifetimeScope = scope;
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

            ActiveHandler.Add(viewModel, request.Mode);

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
    /// A default implementation of <see cref="INavigationStack"/> that simply stores and traverses a linear history.
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
            if (!request.Mode.IgnoreHistory && (HistoryPosition < 0 || request != Requests[HistoryPosition]))
            {
                if (HistoryPosition + 1 != Requests.Count)
                {
                    Requests.RemoveRange(HistoryPosition + 1, Requests.Count - (HistoryPosition + 1));
                }

                Requests.Add(request);
                HistoryPosition++;
                SetCanGo();
            }

            requestStream.EmitValue(request);
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

    /// <summary>
    /// A default implementation of <see cref="INavigationActiveHandler"/> that manages the <see cref="ActiveViewModels"/> using the expected behaviors of each <see cref="NavigationMode"/>.
    /// </summary>
    public class NavigationActiveHandler : INavigationActiveHandler
    {
        /// <inheritdoc/>
        public IEnumerable<IViewModel> ActiveViewModels
            => WindowViewModels.Concat(new IViewModel[] { ShellViewModel, PageViewModel }).Where(v => v != null);

        private IViewModel ShellViewModel { get; set; }

        private IViewModel PageViewModel { get; set; }

        private List<IViewModel> WindowViewModels { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationActiveHandler"/>.
        /// </summary>
        public NavigationActiveHandler()
        {
            WindowViewModels = new List<IViewModel>();
        }

        /// <inheritdoc/>
        public void Add(IViewModel viewModel, NavigationMode mode)
        {
            if(mode.OverlayMode == NavigationOverlay.Override)
            {
                ShellViewModel = viewModel;
            }
            else if(mode.OverlayMode == NavigationOverlay.Page || mode.OverlayMode == NavigationOverlay.Modal)
            {
                //// TODO: Support possibly having both page *and* modal dialog active.
                PageViewModel = viewModel;
            }
            else if(mode.OverlayMode == NavigationOverlay.Window)
            {
                //// TODO: Support for when windows are closed by the user, and how that's handled in navigation.
                WindowViewModels.Add(viewModel);
            }
        }
    }
}
