using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// An <see cref="IBinding{T}"/> expression that represents a constant value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object this <see cref="IBinding{T}"/> represents.</typeparam>
    public class ConstantBinding<T> : Observable, IBinding<T>
    {
        private T storedValue;

        /// <inheritdoc/>
        public T CurrentValue 
        { 
            get => storedValue; 
            set => throw new BindingException("Cannot set the value of a ConstantBinding expression."); 
        }

        /// <inheritdoc/>
        public event EventHandler CurrentValueChanged;

        /// <summary>
        /// Creates a new <see cref="ConstantBinding{T}"/> with the given value.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> value to store.</param>
        public ConstantBinding(T value)
        {
            storedValue = value;
        }
    }
}
