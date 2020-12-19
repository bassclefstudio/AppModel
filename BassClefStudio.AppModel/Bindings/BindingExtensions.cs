﻿using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// Provides extension methods for creating complex <see cref="IBindingExpression{T}"/>s.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Creates an <see cref="IBindingExpression{T}"/> for a property on an existing <see cref="IBindingExpression{T}"/>'s <see cref="IBindingExpression{T}.Value"/> object.
        /// </summary>
        /// <typeparam name="T1">The type of the base object.</typeparam>
        /// <typeparam name="T2">The type of the property to bind.</typeparam>
        /// <param name="me">The base object's <see cref="IBindingExpression{T}"/>.</param>
        /// <param name="getProperty">A <see cref="Func{T, TResult}"/> getting the property on <paramref name="me"/>.</param>
        /// <returns>A new <see cref="IBindingExpression{T}"/> for the property on the base object.</returns>
        public static IBindingExpression<T2> GetProperty<T1, T2>(this IBindingExpression<T1> me, Func<T1, T2> getProperty) where T1 : INotifyPropertyChanged
        {
            return new PropertyBindingExpression<T1, T2>(me.DispatcherService, me, getProperty);
        }

        /// <summary>
        /// Creates an <see cref="IBindingExpression{T}"/> for an existing <see cref="IBindingExpression{T}"/> that applies a <see cref="Func{T, TResult}"/> to the existing <see cref="IBindingExpression{T}.Value"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the base object.</typeparam>
        /// <typeparam name="T2">The type of the transformed object.</typeparam>
        /// <param name="me">The base object's <see cref="IBindingExpression{T}"/>.</param>
        /// <param name="transform">A <see cref="Func{T, TResult}"/> transforming the base object to type <typeparamref name="T2"/>.</param>
        /// <returns>A new <see cref="IBindingExpression{T}"/> for the transformed value of the base object.</returns>
        public static IBindingExpression<T2> Transform<T1, T2>(this IBindingExpression<T1> me, Func<T1, T2> transform)
        {
            return new TransformBindingExpression<T1, T2>(me.DispatcherService, me, transform);
        }

        /// <summary>
        /// Creates an <see cref="IBindingExpression{T}"/> for an existing <see cref="IBindingExpression{T}"/> that applies a <see cref="Func{T, TResult}"/> to an existing <see cref="IBindingExpression{T}.Value"/> collection.
        /// </summary>
        /// <typeparam name="T1">The type of the base collection.</typeparam>
        /// <typeparam name="T2">The type of the transformed collection.</typeparam>
        /// <param name="me">The base collection's <see cref="IBindingExpression{T}"/>.</param>
        /// <param name="transform">A <see cref="Func{T, TResult}"/> transforming the base collection to type <typeparamref name="T2"/>.</param>
        /// <param name="update">An <see cref="Action{T}"/> that updates the transformed <see cref="IBindingExpression{T}.Value"/> based on changes to the base collection.</param>
        /// <returns>A new <see cref="IBindingExpression{T}"/> for the transformed collection.</returns>
        public static IBindingExpression<T2> TransformCollection<T1, T2>(this IBindingExpression<T1> me, Func<T1, T2> transform, Action<NotifyCollectionChangedEventArgs> update) where T1 : INotifyCollectionChanged
        {
            return new CollectionTransformBindingExpression<T1, T2>(me.DispatcherService, me, transform, update);
        }

        /// <summary>
        /// Returns an <see cref="IBindingExpression{T}"/> to this <see cref="IView{T}"/>'s current <see cref="IView{T}.ViewModel"/>. Call this while or after <see cref="IView.Initialize"/> is called.
        /// </summary>
        /// <typeparam name="T">The type of the view-model to bind to.</typeparam>
        /// <param name="dispatcherService">The <see cref="IDispatcherService"/> used to send change notifications.</param>
        /// <param name="view">The <see cref="IView{T}"/> where the view-model could be found.</param>
        /// <returns>An <see cref="IBindingExpression{T}"/> binding to the constant value of the current view-model.</returns>
        public static IBindingExpression<T> ViewModelBinding<T>(this IView<T> view, IDispatcherService dispatcherService) where T : IViewModel
        {
            return new ConstantBindingExpression<T>(dispatcherService, view.ViewModel);
        }
    }
}
