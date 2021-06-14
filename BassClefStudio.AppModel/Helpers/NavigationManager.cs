using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// A default implementation of <see cref="INavigationService"/> that uses factory methods to create the necessary DI resources.
    /// </summary>
    public class NavigationManager : INavigationService
    {
        #region Initialization

        /// <summary>
        /// The injected <see cref="IViewProvider"/> for setting view content.
        /// </summary>
        public IViewProvider ViewProvider { get; set; }

        /// <summary>
        /// The injected <see cref="INavigationStack"/> history stack.
        /// </summary>
        public INavigationStack Stack { get; set; }

        /// <summary>
        /// The injected factory method for creating <see cref="IView"/>s of the given type.
        /// </summary>
        public Func<Type, IView> CreateView { get; set; }

        /// <summary>
        /// The injected factory method for creating <see cref="IViewModel"/>s of the given type.
        /// </summary>
        public Func<Type, IViewModel> CreateViewModel { get; set; }

        #endregion
        #region Methods

        /// <inheritdoc/>
        public void Initialize()
        {
            ViewProvider.StartUI();
            Stack.Clear();
        }

        /// <inheritdoc/>
        public void Navigate(NavigationRequest request)
        {
            var viewType = typeof(IView<>).MakeGenericType(request.ViewModelType);
            var viewModel = CreateViewModel(request.ViewModelType);
            var view = CreateView(viewType);
            ViewProvider.SetView(view);
            viewType.GetProperty("ViewModel").SetValue(view, viewModel);
            SynchronousTask initViewModelTask =
                new SynchronousTask(() => viewModel.InitializeAsync(request.Parameter));
            initViewModelTask.RunTask();
            view.Initialize();
            Stack.AddRequest(request);
        }

        #endregion
    }
}
