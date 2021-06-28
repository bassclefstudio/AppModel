using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Provides a full, multi-layered navigation history to an <see cref="INavigationService"/>.
    /// </summary>
    public interface INavigationHistory : ITraversable
    {
        /// <summary>
        /// An <see cref="IStream{T}"/> of unique <see cref="NavigationRequest"/>s either provided to or emitted from this <see cref="INavigationHistory"/>.
        /// </summary>
        IStream<NavigationRequest> RequestStream { get; }

        /// <summary>
        /// The collection of all <see cref="INavigationLayer"/> layers in the full navigation history.
        /// </summary>
        IEnumerable<INavigationLayer> Layers { get; }

        /// <summary>
        /// The current <see cref="INavigationLayer"/> from <see cref="Layers"/> that the <see cref="INavigationHistory"/> is currently located at. Usually the last ('top') layer.
        /// </summary>
        INavigationLayer CurrentLayer { get; }
    }

    /// <summary>
    /// Represents a single layer of navigation history.
    /// </summary>
    public interface INavigationLayer : ITraversable
    {
        /// <summary>
        /// The current <see cref="IViewModel"/> instance active on this <see cref="INavigationLayer"/>, or 'null' if there isn't any.
        /// </summary>
        IViewModel CurrentViewModel { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether this <see cref="INavigationLayer"/> can be closed/collapsed once its history runs out by a parent <see cref="INavigationHistory"/>.
        /// </summary>
        bool IsReturnable { get; set; }
    }

    /// <summary>
    /// Any service that provides some navigation history which supports backwards and forwards navigation.
    /// </summary>
    public interface ITraversable
    {
        /// <summary>
        /// Erases all built history from this <see cref="ITraversable"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a value indicating whether the <see cref="ITraversable"/> has items in the current back history.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Navigates backwards in the <see cref="ITraversable"/>'s history.
        /// </summary>
        /// <returns>A <see cref="NavigationRequest"/> which will perform the desired action.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="ITraversable"/> cannot currently go back (see <see cref="CanGoBack"/>).</exception>
        NavigationRequest GoBack();

        /// <summary>
        /// Gets a value indicating whether the <see cref="ITraversable"/> has items in the current forward history.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Navigates forwards in the <see cref="ITraversable"/>'s history.
        /// </summary>
        /// <returns>A <see cref="NavigationRequest"/> which will perform the desired action.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="ITraversable"/> cannot currently go forward (see <see cref="CanGoForward"/>).</exception>
        NavigationRequest GoForward();

        /// <summary>
        /// Handles the given navigation request and adds it to this <see cref="INavigationLayer"/>'s history.
        /// </summary>
        /// <param name="request">The <see cref="NavigationRequest"/> request that was made.</param>
        /// <param name="viewModel">The <see cref="IViewModel"/> instance of the view-model that made the request.</param>
        void HandleNavigation(NavigationRequest request, IViewModel viewModel);
    }

    /// <summary>
    /// Provides extension methods for the <see cref="INavigationHistory"/> interface.
    /// </summary>
    public static class HistoryExtensions
    {
        /// <summary>
        /// Retrieves a collection of all active <see cref="IViewModel"/>s in all <see cref="INavigationLayer"/>s of the navigation history.
        /// </summary>
        /// <param name="history">The <see cref="INavigationHistory"/> being queried.</param>
        /// <returns>A collection of all non-null <see cref="INavigationLayer.CurrentViewModel"/>s for all navigation history layers.</returns>
        public static IEnumerable<IViewModel> GetActiveViewModels(this INavigationHistory history)
        {
            return history.Layers.Select(l => l.CurrentViewModel).Where(v => v != null);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the view-model active in the current <see cref="INavigationLayer"/> of the navigation history.
        /// </summary>
        /// <param name="history">The <see cref="INavigationHistory"/> being queried.</param>
        /// <returns>The <see cref="Type"/> of the currently active view-model, or 'null' if no active view-model was found.</returns>
        public static Type GetActiveViewModelType(this INavigationHistory history)
        {
            return history.CurrentLayer.CurrentViewModel?.GetType();
        }
    }

}
