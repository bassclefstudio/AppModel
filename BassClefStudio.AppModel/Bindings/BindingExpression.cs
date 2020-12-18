using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// Represents an object that manages the binding to a property on another object. Implements <see cref="INotifyPropertyChanged"/> notifications and event notify (via <see cref="ValueChanged"/> event).
    /// </summary>
    /// <typeparam name="T">The type of the value this <see cref="IBindingExpression{T}"/> manages.</typeparam>
    public interface IBindingExpression<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// The current value of the <see cref="IBindingExpression{T}"/>.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// An event fired when the <see cref="Value"/> property is updated.
        /// </summary>
        event EventHandler ValueChanged;
    }

    /// <summary>
    /// An abstract class providing an <see cref="Observable"/> base implementation of the <see cref="IBindingExpression{T}"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseBindingExpression<T> : Observable, IBindingExpression<T>
    {
        private T bindingValue;
        /// <inheritdoc/>
        public T Value { get => bindingValue; protected set { Set(ref bindingValue, value); ValueChanged?.Invoke(this, new EventArgs()); } }

        /// <inheritdoc/>
        public event EventHandler ValueChanged;
    }

    /// <summary>
    /// An <see cref="IBindingExpression{T}"/> that references a constant value.
    /// </summary>
    /// <typeparam name="T">The type of the </typeparam>
    public class ConstantBindingExpression<T> : BaseBindingExpression<T>
    {
        /// <summary>
        /// Creates a new <see cref="ConstantBindingExpression{T}"/>.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> value to store.</param>
        public ConstantBindingExpression(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// A <see cref="IBindingExpression{T}"/> that utilizes the <see cref="INotifyPropertyChanged"/> to bind to a property on a root object referenced by another <see cref="IBindingExpression{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the root object.</typeparam>
    /// <typeparam name="TProp">The type of the property that this <see cref="IBindingExpression{T}"/> binds to.</typeparam>
    public class PropertyBindingExpression<T, TProp> : BaseBindingExpression<TProp> where T : INotifyPropertyChanged
    {
        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that returns a <typeparamref name="TProp"/> value from a <typeparamref name="T"/> root object.
        /// </summary>
        public Func<T, TProp> GetProperty { get; }

        /// <summary>
        /// An <see cref="IBindingExpression{T}"/> to the root <typeparamref name="T"/> object.
        /// </summary>
        public IBindingExpression<T> ObjectBinding { get; }

        private T RootObject;

        /// <summary>
        /// Creates a new <see cref="PropertyBindingExpression{T, TProp}"/>.
        /// </summary>
        /// <param name="rootObject">An <see cref="IBindingExpression{T}"/> to the root <typeparamref name="T"/> object.</param>
        /// <param name="getPropertyFunc">A <see cref="Func{T, TResult}"/> that returns a <typeparamref name="TProp"/> value from a <typeparamref name="T"/> root object.</param>
        public PropertyBindingExpression(IBindingExpression<T> rootObject, Func<T, TProp> getPropertyFunc)
        {
            ObjectBinding = rootObject;
            GetProperty = getPropertyFunc;

            ObjectBinding.ValueChanged += RootChanged;
            RootChanged();
        }

        private void RootChanged(object sender, EventArgs e) => RootChanged();
        private void RootChanged()
        {
            if (RootObject != null)
            {
                RootObject.PropertyChanged -= RootPropertyChanged;
            }
            RootObject = ObjectBinding.Value;
            if (RootObject != null)
            {
                RootObject.PropertyChanged += RootPropertyChanged;
            }
        }

        private void RootPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Value = GetProperty(RootObject);
        }
    }

    /// <summary>
    /// An <see cref="IBindingExpression{T}"/> that applies a transformation function to an object referenced by a toot <see cref="IBindingExpression{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the root object.</typeparam>
    /// <typeparam name="TOut">The type of output value this <see cref="IBindingExpression{T}"/> provides.</typeparam>
    public class TransformBindingExpression<T, TOut> : BaseBindingExpression<TOut>
    {
        /// <summary>
        /// An <see cref="IBindingExpression{T}"/> referencing the root object of this <see cref="TransformBindingExpression{T, TOut}"/>.
        /// </summary>
        public IBindingExpression<T> ObjectBinding { get; }

        /// <summary>
        /// A function applied when <see cref="ObjectBinding"/> is updated to convert an object of type <typeparamref name="T"/> to an object of type <typeparamref name="TOut"/>.
        /// </summary>
        public Func<T, TOut> TransformFunc { get; }

        /// <summary>
        /// Creates a new <see cref="TransformBindingExpression{T, TOut}"/>.
        /// </summary>
        /// <param name="objectBinding">An <see cref="IBindingExpression{T}"/> referencing the root object of this <see cref="TransformBindingExpression{T, TOut}"/>.</param>
        /// <param name="transformFunc">A function applied when <see cref="ObjectBinding"/> is updated to convert an object of type <typeparamref name="T"/> to an object of type <typeparamref name="TOut"/>.</param>
        public TransformBindingExpression(IBindingExpression<T> objectBinding, Func<T, TOut> transformFunc)
        {
            ObjectBinding = objectBinding;
            TransformFunc = transformFunc;

            ObjectBinding.ValueChanged += RootChanged;
            RootChanged();
        }

        private void RootChanged(object sender, EventArgs e) => RootChanged();
        private void RootChanged()
        {
            Value = TransformFunc(ObjectBinding.Value);
        }
    }

    /// <summary>
    /// An <see cref="IBindingExpression{T}"/> that applies a transformation function to a collection referenced by a toot <see cref="IBindingExpression{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the root collection, which should post <see cref="INotifyCollectionChanged"/> events.</typeparam>
    /// <typeparam name="TOut">The type of output value this <see cref="IBindingExpression{T}"/> provides.</typeparam>
    public class CollectionTransformBindingExpression<T, TOut> : BaseBindingExpression<TOut> where T : INotifyCollectionChanged
    {
        /// <summary>
        /// An <see cref="IBindingExpression{T}"/> referencing the root object of this <see cref="TransformBindingExpression{T, TOut}"/>.
        /// </summary>
        public IBindingExpression<T> ObjectBinding { get; }

        /// <summary>
        /// A function applied when <see cref="ObjectBinding"/> is updated to convert an object of type <typeparamref name="T"/> to an object of type <typeparamref name="TOut"/>.
        /// </summary>
        public Func<T, TOut> TransformFunc { get; }

        /// <summary>
        /// Called whenever the collection <see cref="ObjectBinding"/> refers to is changed or cleared, allowing for the updating of the existing <typeparamref name="TOut"/> <see cref="BaseBindingExpression{T}.Value"/>.
        /// </summary>
        public Action<NotifyCollectionChangedEventArgs> UpdateFunc { get; }

        private T RootObject;

        /// <summary>
        /// Creates a new <see cref="TransformBindingExpression{T, TOut}"/>.
        /// </summary>
        /// <param name="objectBinding">An <see cref="IBindingExpression{T}"/> referencing the root object of this <see cref="TransformBindingExpression{T, TOut}"/>.</param>
        /// <param name="transformFunc">A function applied when <see cref="ObjectBinding"/> is updated to convert an object of type <typeparamref name="T"/> to an object of type <typeparamref name="TOut"/>.</param>
        public CollectionTransformBindingExpression(IBindingExpression<T> objectBinding, Func<T, TOut> transformFunc, Action<NotifyCollectionChangedEventArgs> updateFunc)
        {
            ObjectBinding = objectBinding;
            TransformFunc = transformFunc;
            UpdateFunc = updateFunc;
            ObjectBinding.ValueChanged += RootChanged;
            RootChanged();
        }

        private void RootChanged(object sender, EventArgs e) => RootChanged();
        private void RootChanged()
        {
            if (RootObject != null)
            {
                RootObject.CollectionChanged -= RootCollectionChanged;
            }
            RootObject = ObjectBinding.Value;
            if (RootObject != null)
            {
                RootObject.CollectionChanged += RootCollectionChanged;
            }

            Value = TransformFunc(RootObject);
        }

        private void RootCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateFunc(e);
        }
    }
}
