using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// A description of how to navigate to a specific state in the app, used by <see cref="INavigationService"/> and related services.
    /// </summary>
    public struct NavigationRequest : IEquatable<NavigationRequest>
    {
        #region Defaults

        /// <summary>
        /// The default <see cref="NavigationRequest"/> for requesting a <see cref="INavigationLayer"/> close/collapse.
        /// </summary>
        public static NavigationRequest Close { get; } = new NavigationRequest(new NavigationProperties(LayerBehavior.Default, HistoryBehavior.Skip));

        #endregion

        /// <summary>
        /// The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.
        /// </summary>
        public object Parameter { get; }

        /// <summary>
        /// The <see cref="NavigationProperties"/> describing the behavior of the navigation operation.
        /// </summary>
        public NavigationProperties Properties { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether this is a 'close request'. If set to 'true', no new navigation will occur, but existing controls and layers will be requested to close.
        /// </summary>
        public bool IsCloseRequest { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="viewModelType">The <see cref="Type"/> of the <see cref="IViewModel"/> being navigated to.</param>
        /// <param name="properties">The <see cref="NavigationProperties"/> describing the behavior of the navigation operation.</param>
        /// <param name="parameter">An <see cref="object"/> parameter being passed to the <see cref="IViewModel.InitializeAsync(object)"/> task when navigation occurs.</param>
        public NavigationRequest(Type viewModelType, NavigationProperties properties, object parameter = null)
        {
            ViewModelType = viewModelType;
            Properties = properties;
            Parameter = parameter;
            IsCloseRequest = false;
        }

        /// <summary>
        /// Creates a new close <see cref="NavigationRequest"/>.
        /// </summary>
        /// <param name="properties">The <see cref="NavigationProperties"/> describing the behavior of the navigation operation.</param>
        public NavigationRequest(NavigationProperties properties)
        {
            ViewModelType = null;
            Properties = properties;
            Parameter = null;
            IsCloseRequest = true;
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
                   EqualityComparer<object>.Default.Equals(Parameter, other.Parameter) &&
                   EqualityComparer<NavigationProperties>.Default.Equals(Properties, other.Properties);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -15731548;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ViewModelType);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Parameter);
            hashCode = hashCode * -1521134295 + Properties.GetHashCode();
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
    public struct NavigationProperties : IEquatable<NavigationProperties>
    {
        #region Defaults

        /// <summary>
        /// The default <see cref="NavigationProperties"/> for navigating between pages.
        /// </summary>
        public static readonly NavigationProperties Default = new NavigationProperties(LayerBehavior.Default, HistoryBehavior.Default);

        /// <summary>
        /// The default <see cref="NavigationProperties"/> for setting up a shell page (navigation UI).
        /// </summary>
        public static readonly NavigationProperties Shell = new NavigationProperties(LayerBehavior.Shell, HistoryBehavior.Block);

        /// <summary>
        /// The default <see cref="NavigationProperties"/> for opening a modal dialog in a new navigation layer.
        /// </summary>
        public static readonly NavigationProperties Dialog = new NavigationProperties(LayerBehavior.Modal, HistoryBehavior.Default);

        #endregion

        /// <summary>
        /// The <see cref="LayerBehavior"/> mode that should be used to present the navigated content.
        /// </summary>
        public LayerBehavior LayerMode { get; }

        /// <summary>
        /// The <see cref="HistoryBehavior"/> mode that should be used by the <see cref="INavigationHistory"/> to handle the navigation stack.
        /// </summary>
        public HistoryBehavior HistoryMode { get; }

        /// <summary>
        /// Defines a new <see cref="NavigationProperties"/>.
        /// </summary>
        /// <param name="overlayMode">The <see cref="LayerBehavior"/> mode that should be used to present the navigated content.</param>
        /// <param name="historyMode">The <see cref="HistoryBehavior"/> mode that should be used by the <see cref="INavigationHistory"/> to handle the navigation stack.</param>
        public NavigationProperties(LayerBehavior overlayMode, HistoryBehavior historyMode)
        {
            LayerMode = overlayMode;
            HistoryMode = historyMode;
        }

        #region Operators

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is NavigationProperties mode && Equals(mode);
        }

        /// <inheritdoc/>
        public bool Equals(NavigationProperties other)
        {
            return LayerMode == other.LayerMode &&
                   HistoryMode == other.HistoryMode;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 360113769;
            hashCode = hashCode * -1521134295 + LayerMode.GetHashCode();
            hashCode = hashCode * -1521134295 + HistoryMode.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(NavigationProperties left, NavigationProperties right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(NavigationProperties left, NavigationProperties right)
        {
            return !(left == right);
        }

        #endregion
    }

    /// <summary>
    /// Describes how the <see cref="INavigationLayer"/>s of navigation history should behave during/after navigation.
    /// </summary>
    public enum LayerBehavior
    {
        /// <summary>
        /// No new layer is created before or after navigation. Default for page navigation and other operations.
        /// </summary>
        Default = 0,
        /// <summary>
        /// A new layer is created to contain the navigated content, which will appear in front of existing content in a modal display/dialog.
        /// </summary>
        Modal = 1,
        /// <summary>
        /// A new navigation layer is created after navigation, and additional navigation will occur within the currently navigated content. Ideal for 'shell' pages with navigation headers and sticky content.
        /// </summary>
        Shell = 2
    }

    /// <summary>
    /// Describes how <see cref="ITraversable"/>s should handle this content in their navigation stacks.
    /// </summary>
    public enum HistoryBehavior
    { 
        /// <summary>
        /// The navigation request is added to the back navigation stack, and can be returned to.
        /// </summary>
        Default = 0,
        /// <summary>
        /// This is transient content within a navigation layer, and back navigation should skip over (ignore) this navigation request.
        /// </summary>
        Skip = 1,
        /// <summary>
        /// The navigation to this content hides all previous navigation history on this layer. Previous content cannot be returned to. Ideal for initial, fixed content like headers and navigation.
        /// </summary>
        Block = 2
    }
}
