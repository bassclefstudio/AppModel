using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// An extension to the <see cref="App"/> DI container that produces and manages <see cref="IViewModel"/>s for navigation.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// The <see cref="INavigationHistory"/> history this <see cref="INavigationService"/> consumes. Used for extending the service to provide back navigation (see <see cref="NavigationServiceExtensions.GoBack(INavigationService)"/>).
        /// </summary>
        INavigationHistory History { get; }

        /// <summary>
        /// Navigates to an <see cref="IViewModel"/> using the given <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="request">The <see cref="NavigationRequest"/> describing how to navigate to the new view-model.</param>
        /// <param name="includeHistory">A <see cref="bool"/> indicating whether it's necessary for the <see cref="INavigationService"/> to pass this request on to the attached <see cref="History"/> stack (generally only set to 'false' if the <see cref="INavigationHistory"/> already recorded the request).</param>
        void Navigate(NavigationRequest request, bool includeHistory = true);
    }

    /// <summary>
    /// Provides extension methods for the <see cref="INavigationService"/> interface.
    /// </summary>
    public static class NavigationServiceExtensions
    {
        #region Navigation

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate(this INavigationService navigationService, Type viewModelType, object parameter = null) => navigationService.Navigate(viewModelType, NavigationProperties.Default, parameter);

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="navigationMode">The <see cref="NavigationProperties"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate(this INavigationService navigationService, Type viewModelType, NavigationProperties navigationMode, object parameter = null) => navigationService.Navigate(new NavigationRequest(viewModelType, navigationMode, parameter));

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <typeparam name="T">The type of the <see cref="IViewModel"/> being navigated to.</typeparam>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate<T>(this INavigationService navigationService, object parameter = null) => navigationService.Navigate(typeof(T), NavigationProperties.Default, parameter);

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <typeparam name="T">The type of the <see cref="IViewModel"/> being navigated to.</typeparam>
        /// <param name="navigationMode">The <see cref="NavigationProperties"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate<T>(this INavigationService navigationService, NavigationProperties navigationMode, object parameter = null) => navigationService.Navigate(new NavigationRequest(typeof(T), navigationMode, parameter));

        #endregion
        #region History

        /// <summary>
        /// Clears the entire <see cref="INavigationService"/>'s history.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the operation.</param>
        public static void ClearHistory(this INavigationService navigationService)
        {
            navigationService.History.Clear();
        }

        /// <summary>
        /// Navigates backwards in the <see cref="INavigationService"/> history.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <exception cref="InvalidOperationException">The <see cref="INavigationService"/> cannot currently go back (see <see cref="ITraversable.CanGoBack"/>).</exception>
        public static void GoBack(this INavigationService navigationService)
        {
            navigationService.Navigate(navigationService.History.GoBack(), false);
        }

        /// <summary>
        /// Navigates forwards in the <see cref="INavigationService"/> history.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <exception cref="InvalidOperationException">The <see cref="INavigationService"/> cannot currently go forward (see <see cref="ITraversable.CanGoForward"/>).</exception>
        public static void GoForward(this INavigationService navigationService)
        {
            navigationService.Navigate(navigationService.History.GoForward(), false);
        }

        #endregion
    }
}
