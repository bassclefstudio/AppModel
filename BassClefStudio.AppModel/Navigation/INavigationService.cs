using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// An extension to the <see cref="App"/> DI container that produces and manages navigating between <see cref="IViewModel"/>s.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> using the given <see cref="NavigationRequest"/>.
        /// </summary>
        void Navigate(NavigationRequest request);
    }

    /// <summary>
    /// Provides extension methods for the <see cref="INavigationService"/> interface.
    /// </summary>
    public static class NavigationServiceExtensions
    {
        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate(this INavigationService navigationService, Type viewModelType, object parameter = null) => navigationService.Navigate(viewModelType, NavigationMode.Default, parameter);

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of the given <see cref="Type"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="navigationMode">The <see cref="NavigationMode"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate(this INavigationService navigationService, Type viewModelType, NavigationMode navigationMode, object parameter = null) => navigationService.Navigate(new NavigationRequest(viewModelType, navigationMode, parameter));

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <typeparam name="T">The type of the <see cref="IViewModel"/> being navigated to.</typeparam>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate<T>(this INavigationService navigationService, object parameter = null) => navigationService.Navigate(typeof(T), NavigationMode.Default, parameter);

        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <typeparam name="T">The type of the <see cref="IViewModel"/> being navigated to.</typeparam>
        /// <param name="navigationMode">The <see cref="NavigationMode"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate<T>(this INavigationService navigationService, NavigationMode navigationMode, object parameter = null) => navigationService.Navigate(new NavigationRequest(typeof(T), navigationMode, parameter));

        /// <summary>
        /// Navigates to an <see cref="IViewModel"/> instance.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="viewModel">The instance of the <see cref="IViewModel"/> to navigate to.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate(this INavigationService navigationService, IViewModel viewModel, object parameter = null) => navigationService.Navigate(viewModel, NavigationMode.Default, parameter);

        /// <summary>
        /// Navigates to an <see cref="IViewModel"/> instance.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="viewModel">The instance of the <see cref="IViewModel"/> to navigate to.</param>
        /// <param name="navigationMode">The <see cref="NavigationMode"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public static void Navigate(this INavigationService navigationService, IViewModel viewModel, NavigationMode navigationMode, object parameter = null) => navigationService.Navigate(new NavigationRequest(viewModel, navigationMode, parameter));

        /// <summary>
        /// <see cref="INavigationService.Navigate(NavigationRequest)"/>s backwards using the <see cref="INavigationStack.GoBack"/> navigation request.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="stack">The <see cref="INavigationStack"/> to use for navigation history.</param>
        public static void GoBack(this INavigationService navigationService, INavigationStack stack)
        {
            var request = stack.GoBack();
            navigationService.Navigate(request);
        }

        /// <summary>
        /// <see cref="INavigationService.Navigate(NavigationRequest)"/>s forwards using the <see cref="INavigationStack.GoForward"/> navigation request.
        /// </summary>
        /// <param name="navigationService">The <see cref="INavigationService"/> performing the navigation operation.</param>
        /// <param name="stack">The <see cref="INavigationStack"/> to use for navigation history.</param>
        public static void GoForward(this INavigationService navigationService, INavigationStack stack)
        {
            var request = stack.GoBack();
            navigationService.Navigate(request);
        }
    }
}
