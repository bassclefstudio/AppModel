using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// A default implementation of <see cref="INavigationService"/>/<see cref="IBackHandler"/> that extends the <see cref="App"/>'s <see cref="ILifetimeScope"/>.
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
        /// Creates a new <see cref="NavigationManager"/> from the provided services.
        /// </summary>
        public NavigationManager(IViewProvider viewProvider, INavigationStack stack, ILifetimeScope scope)
        {
            ViewProvider = viewProvider;
            Stack = stack;
            LifetimeScope = scope;
        }

        #endregion
        #region Methods

        /// <inheritdoc/>
        public bool Initialize()
        {
            ViewProvider.StartUI();
            Stack.Clear();
            return true;
        }

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
}
