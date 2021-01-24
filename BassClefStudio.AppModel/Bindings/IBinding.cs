using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// Represents a reference to some <typeparamref name="T"/> value, with dynamic value-change notifications and direct get/set accessors.
    /// </summary>
    /// <typeparam name="T">The type of the object that this <see cref="IBinding{T}"/> represents.</typeparam>
    public interface IBinding<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Get or set the currently stored <typeparamref name="T"/> value this <see cref="IBinding{T}"/> represents.
        /// </summary>
        T StoredValue { get; set; }

        /// <summary>
        /// An event fired whenever the value of <see cref="StoredValue"/> changes. Equivalent to the <see cref="INotifyPropertyChanged"/> events from the <see cref="StoredValue"/> property.
        /// </summary>
        event EventHandler ValueChanged;
    }

    /// <summary>
    /// Represents an abstract implementation of <see cref="IBinding{T}"/> that supports get-based, one-way data binding.
    /// </summary>
    /// <typeparam name="T">The type of the object that this <see cref="IBinding{T}"/> represents.</typeparam>
    public abstract class OneWayBinding<T> : Observable, IBinding<T>
    {
        /// <summary>
        /// Backing field for <see cref="OneWayBinding{T}"/>'s implementation of the <see cref="StoredValue"/> property.
        /// </summary>
        protected T storedValue;

        /// <inheritdoc/>
        public virtual T StoredValue 
        { 
            get => storedValue; 
            set => throw new BindingException("OneWayBinding<T> does not support setting of the bound value."); 
        }

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Gets the current value to update this <see cref="IBinding{T}"/>'s <see cref="StoredValue"/>.
        /// </summary>
        /// <returns></returns>
        public abstract T GetValue();

        /// <summary>
        /// Calling this method causes the <see cref="OneWayBinding{T}"/> to update itself, triggering <see cref="ValueChanged"/> and similar events.
        /// </summary>
        public void UpdateBinding()
        {
            var newVal = GetValue();
            if(newVal == null || !newVal.Equals(StoredValue))
            {
                Set(ref storedValue, newVal, nameof(StoredValue));
                ValueChanged?.Invoke(this, new EventArgs());
            }
        }
    }

    /// <summary>
    /// Represents an abstract implementation of <see cref="IBinding{T}"/> that supports get- and set-based, two-way data binding.
    /// </summary>
    /// <typeparam name="T">The type of the object that this <see cref="IBinding{T}"/> represents.</typeparam>
    public abstract class TwoWayBinding<T> : OneWayBinding<T>
    {
        /// <inheritdoc/>
        public override T StoredValue
        {
            get => storedValue;
            set 
            {
                SetValue(value);
                UpdateBinding();
            }
        }

        /// <summary>
        /// Sets the backing store for this <see cref="TwoWayBinding{T}"/> to a given <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="newVal">The <typeparamref name="T"/> value to set.</param>
        public abstract void SetValue(T newVal);
    }
}
