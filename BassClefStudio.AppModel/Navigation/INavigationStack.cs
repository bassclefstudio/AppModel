using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents some model of navigated content that allows for the management of a back-stack/history for navigation that is used by the <see cref="INavigationService"/> of an app.
    /// </summary>
    public interface INavigationStack
    {
        /// <summary>
        /// An <see cref="IStream{T}"/> that represents all of the <see cref="NavigationRequest"/> requests this <see cref="INavigationStack"/> handles.
        /// </summary>
        IStream<NavigationRequest> RequestStream { get; }

        /// <summary>
        /// Handles the new <see cref="NavigationRequest"/> request that has been sent by the app, adding it to history.
        /// </summary>
        /// <param name="request">The <see cref="NavigationRequest"/> describing the location and behavior of the most recent navigation operation.</param>
        void AddRequest(NavigationRequest request);

        /// <summary>
        /// Clears the entire <see cref="INavigationStack"/>'s history.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets a value indicating whether the <see cref="INavigationStack"/> has items in the current back history.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Navigates backwards in the <see cref="INavigationStack"/> history.
        /// </summary>
        /// <returns>A <see cref="NavigationRequest"/> describing the operation required to adjust the app's view to the requrested state.</returns>
        NavigationRequest GoBack();

        /// <summary>
        /// Gets a value indicating whether the <see cref="INavigationStack"/> has items in the current forward history.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Navigates forwards in the <see cref="INavigationStack"/> history.
        /// </summary>
        /// <returns>A <see cref="NavigationRequest"/> describing the operation required to adjust the app's view to the requrested state.</returns>
        NavigationRequest GoForward();
    }
}
