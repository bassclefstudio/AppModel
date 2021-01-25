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
        T CurrentValue { get; set; }

        /// <summary>
        /// An event fired whenever the value of <see cref="CurrentValue"/> changes. Equivalent to the <see cref="INotifyPropertyChanged"/> events from the <see cref="CurrentValue"/> property.
        /// </summary>
        event EventHandler CurrentValueChanged;
    }

    /// <summary>
    /// Represents an abstract implementation of <see cref="IBinding{T}"/> that supports get- and set- based, one- or two-way data binding.
    /// </summary>
    /// <typeparam name="T">The type of the object that this <see cref="IBinding{T}"/> represents.</typeparam>
    public abstract class Binding<T> : Observable, IBinding<T>
    {
        /// <summary>
        /// Backing field for <see cref="Binding{T}"/>'s implementation of the <see cref="CurrentValue"/> property.
        /// </summary>
        protected T currentValue;

        /// <inheritdoc/>
        public virtual T CurrentValue 
        { 
            get => currentValue;
            set
            {
                SetValue(value);
                UpdateBinding();
            }
        }

        /// <inheritdoc/>
        public event EventHandler CurrentValueChanged;

        /// <summary>
        /// Gets the current value to update this <see cref="IBinding{T}"/>'s <see cref="CurrentValue"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract T GetValue();

        /// <summary>
        /// Sets the backing store for this <see cref="IBinding{T}"/> to a given <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="newVal">The <typeparamref name="T"/> value to set.</param>
        protected abstract void SetValue(T newVal);

        /// <summary>
        /// Calling this method causes the <see cref="Binding{T}"/> to update itself, triggering <see cref="CurrentValueChanged"/> and similar events.
        /// </summary>
        public void UpdateBinding()
        {
            var newVal = GetValue();
            if(newVal == null || !newVal.Equals(CurrentValue))
            {
                Set(ref currentValue, newVal, nameof(CurrentValue));
                CurrentValueChanged?.Invoke(this, new EventArgs());
            }
        }
        
        /// <summary>
        /// Triggers the <see cref="CurrentValueChanged"/> and <see cref="INotifyPropertyChanged.PropertyChanged"/> events, even if the value of the <see cref="CurrentValue"/> has not changed.
        /// </summary>
        protected void TriggerValueChanged()
        {
            Set(ref currentValue, currentValue, nameof(CurrentValue));
            CurrentValueChanged?.Invoke(this, new EventArgs());
        }
    }
}
