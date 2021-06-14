using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// A service that controls an <see cref="INavigationStack"/> and platform-specific <see cref="IViewProvider"/>s to navigate between <see cref="IViewModel"/> models and serve relevant <see cref="IView"/> UI to the user.
    /// </summary>
    public interface INavigationService : IInitializationHandler
    {
        /// <summary>
        /// Navigates to a <see cref="IViewModel"/> using the given <see cref="NavigationRequest"/>.
        /// </summary>
        void Navigate(NavigationRequest request);
    }

    /// <summary>
    /// A description of how to navigate to a specific state in the app, used by <see cref="INavigationService"/> and stored in the <see cref="INavigationStack"/>'s history.
    /// </summary>
    public struct NavigationRequest
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.
        /// </summary>
        public Type ViewModelType { get; set; }

        /// <summary>
        /// An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// The <see cref="NavigationMode"/> describing the behavior of the navigation operation.
        /// </summary>
        public NavigationMode Mode { get; set; }

        /// <summary>
        /// Creates a new <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="mode">The <see cref="NavigationMode"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public NavigationRequest(Type viewModelType, NavigationMode mode, object parameter = null)
        {
            ViewModelType = viewModelType;
            Mode = mode;
            Parameter = parameter;
        }
    }

    /// <summary>
    /// Provides information about exactly how a navigation event should occur.
    /// </summary>
    public struct NavigationMode
    {
        /// <summary>
        /// The default <see cref="NavigationMode"/> for navigating between pages.
        /// </summary>
        public static NavigationMode Default { get; } = new NavigationMode(NavigationOverlay.Replace);

        /// <summary>
        /// The <see cref="NavigationOverlay"/> mode that should be used to present the navigated content.
        /// </summary>
        public NavigationOverlay OverlayMode { get; }
        
        /// <summary>
        /// If set to 'true', this indicates that the content being navigated to is transient (e.g. a login screen, one-time notification dialog, etc.) and should generally not be included in the <see cref="INavigationStack"/>.
        /// </summary>
        public bool IgnoreHistory { get; }

        /// <summary>
        /// Defines a new <see cref="NavigationMode"/>.
        /// </summary>
        /// <param name="overlayMode">The <see cref="NavigationOverlay"/> mode that should be used to present the navigated content.</param>
        /// <param name="ignoreHistory">If set to 'true', this indicates that the content being navigated to is transient (e.g. a login screen, one-time notification dialog, etc.) and should generally not be included in the <see cref="INavigationStack"/>.</param>
        public NavigationMode(NavigationOverlay overlayMode, bool ignoreHistory = false)
        {
            OverlayMode = overlayMode;
            IgnoreHistory = ignoreHistory;
        }
    }

    /// <summary>
    /// The type of overlay that should be produced when the new content is navigated to.
    /// </summary>
    public enum NavigationOverlay
    {
        /// <summary>
        /// Fully replaces the existing content with the navigated content.
        /// </summary>
        Replace = 0,
        /// <summary>
        /// Displays the navigated content on top of the existing content (in a dialog or modal control).
        /// </summary>
        Modal = 1,
        /// <summary>
        /// Creates a new, movable window with the navigated content.
        /// </summary>
        Window = 2
    }
}
