using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// An <see cref="IBinding{T}"/> that is only changed by the setter on <see cref="IBinding{T}.CurrentValue"/>.
    /// </summary>
    /// <typeparam name="T">The type of object this <see cref="IBinding{T}"/> represents.</typeparam>
    public class SetterBinding<T> : Binding<T>
    {
        /// <summary>
        /// Creates a new default <see cref="SetterBinding{T}"/>.
        /// </summary>
        public SetterBinding()
        {
            currentValue = default(T);
            UpdateBinding();
        }

        /// <summary>
        /// Creates a new <see cref="SetterBinding{T}"/> with the given value.
        /// </summary>
        /// <param name="initialValue">The initial <typeparamref name="T"/> value of the <see cref="IBinding{T}.CurrentValue"/>.</param>
        public SetterBinding(T initialValue)
        {
            currentValue = initialValue;
            UpdateBinding();
        }

        /// <inheritdoc/>
        protected override T GetValue() => currentValue;

        /// <inheritdoc/>
        protected override void SetValue(T newVal) => currentValue = newVal;
    }
}
