using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// A description of how to navigate to a specific state in the app, used by <see cref="INavigationService"/> and stored in the <see cref="INavigationStack"/>'s history.
    /// </summary>
    public struct NavigationRequest : IEquatable<NavigationRequest>
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// An instance of an <see cref="IViewModel"/> to navigate to.
        /// </summary>
        public IViewModel ViewModelInstance { get; }

        /// <summary>
        /// A <see cref="bool"/> that, if set to 'true', indicates that a <see cref="ViewModelInstance"/> has been specified for this navigation operation.
        /// </summary>
        public bool ContainsInstance => ViewModelInstance != null;

        /// <summary>
        /// An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.
        /// </summary>
        public object Parameter { get; }

        /// <summary>
        /// The <see cref="NavigationMode"/> describing the behavior of the navigation operation.
        /// </summary>
        public NavigationMode Mode { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="mode">The <see cref="NavigationMode"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public NavigationRequest(Type viewModelType, NavigationMode mode, object parameter = null)
        {
            ViewModelType = viewModelType;
            ViewModelInstance = null;
            Mode = mode;
            Parameter = parameter;
        }

        /// <summary>
        /// Creates a new instance-based <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="viewModel">An instance of an <see cref="IViewModel"/> to navigate to.</param>
        /// <param name="mode">The <see cref="NavigationMode"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public NavigationRequest(IViewModel viewModel, NavigationMode mode, object parameter = null)
        {
            ViewModelType = null;
            ViewModelInstance = viewModel;
            Mode = mode;
            Parameter = parameter;
        }

        #region Operators

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is NavigationRequest request && Equals(request);
        }

        /// <inheritdoc/>
        public bool Equals(NavigationRequest other)
        {
            return EqualityComparer<Type>.Default.Equals(ViewModelType, other.ViewModelType) &&
                   EqualityComparer<IViewModel>.Default.Equals(ViewModelInstance, other.ViewModelInstance) &&
                   ContainsInstance == other.ContainsInstance &&
                   EqualityComparer<object>.Default.Equals(Parameter, other.Parameter) &&
                   EqualityComparer<NavigationMode>.Default.Equals(Mode, other.Mode);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -15731548;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ViewModelType);
            hashCode = hashCode * -1521134295 + EqualityComparer<IViewModel>.Default.GetHashCode(ViewModelInstance);
            hashCode = hashCode * -1521134295 + ContainsInstance.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Parameter);
            hashCode = hashCode * -1521134295 + Mode.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(NavigationRequest left, NavigationRequest right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(NavigationRequest left, NavigationRequest right)
        {
            return !(left == right);
        }

        #endregion
    }

    /// <summary>
    /// Provides information about exactly how a navigation event should occur.
    /// </summary>
    public struct NavigationMode : IEquatable<NavigationMode>
    {
        #region Defaults

        /// <summary>
        /// The default <see cref="NavigationMode"/> for navigating between pages.
        /// </summary>
        public static NavigationMode Default { get; } = new NavigationMode(NavigationOverlay.Page);

        /// <summary>
        /// The default <see cref="NavigationMode"/> for setting up a shell page (navigation UI).
        /// </summary>
        public static NavigationMode Shell { get; } = new NavigationMode(NavigationOverlay.Override, true);

        #endregion

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

        #region Operators

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is NavigationMode mode && Equals(mode);
        }

        /// <inheritdoc/>
        public bool Equals(NavigationMode other)
        {
            return OverlayMode == other.OverlayMode &&
                   IgnoreHistory == other.IgnoreHistory;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 360113769;
            hashCode = hashCode * -1521134295 + OverlayMode.GetHashCode();
            hashCode = hashCode * -1521134295 + IgnoreHistory.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(NavigationMode left, NavigationMode right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(NavigationMode left, NavigationMode right)
        {
            return !(left == right);
        }

        #endregion
    }

    /// <summary>
    /// The type of overlay that should be produced when the new content is navigated to.
    /// </summary>
    public enum NavigationOverlay
    {
        /// <summary>
        /// Requests the default page container display the navigated content.
        /// </summary>
        Page = 0,
        /// <summary>
        /// Fully replaces the existing content with the navigated content. Mainly used for setting shell (navigation) content.
        /// </summary>
        Override = 1,
        /// <summary>
        /// Displays the navigated content on top of the existing content (in a dialog or modal control).
        /// </summary>
        Modal = 2,
        /// <summary>
        /// Creates a new, movable window with the navigated content.
        /// </summary>
        Window = 3
    }
}
