using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a service that can navigate between <see cref="IView{T}"/>s in a platform-specific way. This interface is generally for internal use, and for most navigation apps should use the methods available on the <see cref="App"/> class instead, as they provide additional functionality.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Called when the app has been activated with UI, this method should enable the UI, build windows, and start this <see cref="INavigationService"/>'s navigation context.
        /// </summary>
        void InitializeNavigation();

        /// <summary>
        /// Navigates to the given <see cref="IView"/> view, displaying its content to the user.
        /// </summary>
        /// <param name="view">The instance of the <see cref="IView"/> to navigate to.</param>
        void Navigate(IView view);

        /// <summary>
        /// A <see cref="bool"/> indicating whether this <see cref="INavigationService"/> supports back navigation in its current state. 
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Initiate back navigation and navigate to the previously visited <see cref="IView"/> view.
        /// </summary>
        /// <returns>The <see cref="IView"/> that has been navigated to.</returns>
        IView GoBack();
    }

    /// <summary>
    /// Represents a base <see cref="INavigationService"/> that navigates between views of type <typeparamref name="T"/> and implements stack-based back navigation.
    /// </summary>
    /// <typeparam name="T">The type of views this <see cref="NavigationService{T}"/> navigates between.</typeparam>
    public abstract class NavigationService<T> : INavigationService
    {
        /// <inheritdoc/>
        public bool CanGoBack => NavigationStack.Count > 1;

        /// <summary>
        /// The <see cref="Stack{T}"/> containing back navigation information.
        /// </summary>
        protected Stack<T> NavigationStack { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationService{T}"/>.
        /// </summary>
        public NavigationService()
        {
            NavigationStack = new Stack<T>();
        }

        /// <inheritdoc/>
        public abstract void InitializeNavigation();

        /// <summary>
        /// Navigates to a <typeparamref name="T"/> view internally, using the platform-specific navigation APIs.
        /// </summary>
        /// <param name="view">The <see cref="IView"/> and <typeparamref name="T"/> to navigate to.</param>
        protected abstract void NavigateInternal(T view);

        /// <inheritdoc/>
        public void Navigate(IView view)
        {
            if(view is T tView)
            {
                NavigateInternal(tView);
                NavigationStack.Push(tView);
            }
            else
            {
                throw new ArgumentException($"Navigation only supports views of type \"{typeof(T).Name}\"; view is of type \"{view?.GetType().Name}\".");
            }
        }

        /// <inheritdoc/>
        public IView GoBack()
        {
            if (CanGoBack)
            {
                NavigationStack.Pop();
                var view = NavigationStack.Peek();
                NavigateInternal(view);
                return (view as IView);
            }
            else
            {
                throw new InvalidOperationException("Cannot initiate back navigation - CanGoBack is currently 'false'.");
            }
        }
    }
}
