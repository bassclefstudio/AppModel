﻿using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// A default implementation of <see cref="INavigationService"/> that extends the <see cref="App"/>'s <see cref="ILifetimeScope"/>.
    /// </summary>
    public class NavigationService : INavigationService
    {
        #region Initialization

        /// <summary>
        /// The injected <see cref="IViewProvider"/> for setting view content.
        /// </summary>
        protected IViewProvider ViewProvider { get; }

        /// <inheritdoc/>
        public INavigationHistory History { get; }

        /// <summary>
        /// The <see cref="ILifetimeScope"/> delegated by the <see cref="App"/> to the <see cref="NavigationService"/> to resolve view and view-model instances.
        /// </summary>
        protected ILifetimeScope LifetimeScope { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationService"/> from the provided services.
        /// </summary>
        public NavigationService(IViewProvider viewProvider, INavigationHistory history, ILifetimeScope scope)
        {
            ViewProvider = viewProvider;
            History = history;
            LifetimeScope = scope;
        }

        #endregion
        #region Methods

        /// <inheritdoc/>
        public void Navigate(NavigationRequest request, bool includeHistory = true)
        {
            if (request.IsCloseRequest)
            {
                //// Send close request to IViewProvider.
                ViewProvider.SetView(request, null);
                if (includeHistory)
                {
                    //// Reports request to history.
                    History.HandleNavigation(request, null);
                }
            }
            else
            {
                //// Resolves values from the request.
                IViewModel viewModel = (IViewModel)LifetimeScope.Resolve(request.ViewModelType);
                Type viewModelRealizedType = viewModel.GetType();
                Type viewType = typeof(IView<>).MakeGenericType(viewModelRealizedType);
                IView view = (IView)LifetimeScope.Resolve(viewType, new TypedParameter(viewModelRealizedType, viewModel));

                //// Sets the view.
                ViewProvider.SetView(request, view);

                if (includeHistory)
                {
                    //// Reports navigated content to history.
                    History.HandleNavigation(request, viewModel);
                }

                //// Initializes the view-model and view.
                view.Initialize();
                SynchronousTask initViewModelTask =
                    new SynchronousTask(() => viewModel.InitializeAsync(request.Parameter));
                initViewModelTask.RunTask();
            }
        }

        #endregion
    }
}
