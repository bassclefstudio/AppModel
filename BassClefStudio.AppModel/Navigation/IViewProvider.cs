using Autofac;
using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a service that can set the <see cref="IView{T}"/> content of the app in a platform-specific way. This interface is generally for internal use, and for most navigation apps should use the methods available on the <see cref="INavaigationService"/> interface instead, as they provide additional functionality.
    /// </summary>
    public interface IViewProvider
    {
        /// <summary>
        /// Called when the app has been activated with UI, this method should enable the UI, build windows, and start this <see cref="IViewProvider"/>'s navigation context.
        /// </summary>
        void InitializeNavigation();

        /// <summary>
        /// Sets the content of the app to an <see cref="IView"/> view.
        /// </summary>
        /// <param name="view">The instance of the <see cref="IView"/> to navigate to.</param>
        void SetView(IView view);
    }

    /// <summary>
    /// A default implementation of <see cref="IViewProvider"/> that manages <typeparamref name="T"/> views.
    /// </summary>
    /// <typeparam name="T">The type of all <see cref="IView"/>s that thi s<see cref="ViewProvider{T}"/> supports.</typeparam>
    public abstract class ViewProvider<T> : IViewProvider
    {
        /// <inheritdoc/>
        public abstract void InitializeNavigation();

        /// <summary>
        /// Internally sets the content of the app to an <typeparamref name="T"/> view.
        /// </summary>
        /// <param name="view"></param>
        protected abstract void SetViewInternal(T view);

        /// <inheritdoc/>
        public void SetView(IView view)
        {
            if(view is T tView)
            {
                SetViewInternal(tView);
            }
            else
            {
                throw new ArgumentException($"This ViewProvider does not support setting views of type {view?.GetType()}.", "view");
            }
        }
    }
}
