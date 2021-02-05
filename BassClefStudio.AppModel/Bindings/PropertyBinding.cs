﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// Represents a one-way <see cref="IBinding{T}"/> between a parent object and a property.
    /// </summary>
    /// <typeparam name="TIn">The type of the <see cref="ParentObject"/>.</typeparam>
    /// <typeparam name="TOut">The type of the <see cref="IBinding{T}.CurrentValue"/>.</typeparam>
    public class PropertyBinding<TIn, TOut> : Binding<TOut>
    {
        /// <summary>
        /// An <see cref="IBinding{T}"/> to the parent object from which the property is retrieved.
        /// </summary>
        public IBinding<TIn> ParentObject { get; }

        /// <summary>
        /// A function that gets the <typeparamref name="TOut"/> property from a <typeparamref name="TIn"/> parent.
        /// </summary>
        public Func<TIn, TOut> GetPropertyFunc { get; }

        /// <summary>
        /// A method that sets the <typeparamref name="TOut"/> property of a <typeparamref name="TIn"/> parent.
        /// </summary>
        public Action<TIn, TOut> SetPropertyAction { get; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether this <see cref="PropertyBinding{TIn, TOut}"/> should continue to call <see cref="GetPropertyFunc"/> and <see cref="SetPropertyAction"/> if the <see cref="ParentObject"/> is known to currently represent a 'null' value.
        /// </summary>
        public bool NullAllowed { get; set; }

        /// <summary>
        /// The name of the <typeparamref name="TOut"/> property, which can be used to selectively trigger <see cref="Binding{T}.UpdateBinding"/> only when a property of this name is changed on the <see cref="ParentObject"/>.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Creates a new get-only <see cref="PropertyBinding{TIn, TOut}"/>.
        /// </summary>
        /// <param name="parentObject">An <see cref="IBinding{T}"/> to the parent object from which the property is retrieved.</param>
        /// <param name="getProperty">A function that gets the <typeparamref name="TOut"/> property from a <typeparamref name="TIn"/> parent.</param>
        /// <param name="propertyName">The name of the <typeparamref name="TOut"/> property, which can be used to selectively trigger <see cref="Binding{T}.UpdateBinding"/> only when a property of this name is changed on the <see cref="ParentObject"/>.</param>
        /// <param name="nullAllowed">A <see cref="bool"/> indicating whether this <see cref="PropertyBinding{TIn, TOut}"/> should continue to call <see cref="GetPropertyFunc"/> and <see cref="SetPropertyAction"/> if the <see cref="ParentObject"/> is known to currently represent a 'null' value.</param>
        public PropertyBinding(IBinding<TIn> parentObject, Func<TIn, TOut> getProperty, string propertyName = null, bool nullAllowed = false)
            : this(parentObject, getProperty, (i, o) => throw new BindingException("This is a get-only property binding. Setting the value of this property is not supported."), propertyName, nullAllowed)
        { }

        /// <summary>
        /// Creates a new <see cref="PropertyBinding{TIn, TOut}"/>.
        /// </summary>
        /// <param name="parentObject">An <see cref="IBinding{T}"/> to the parent object from which the property is retrieved.</param>
        /// <param name="getProperty">A function that gets the <typeparamref name="TOut"/> property from a <typeparamref name="TIn"/> parent.</param>
        /// <param name="setProperty">A method that sets the <typeparamref name="TOut"/> property of a <typeparamref name="TIn"/> parent.</param>
        /// <param name="propertyName">The name of the <typeparamref name="TOut"/> property, which can be used to selectively trigger <see cref="Binding{T}.UpdateBinding"/> only when a property of this name is changed on the <see cref="ParentObject"/>.</param>
        /// <param name="nullAllowed">A <see cref="bool"/> indicating whether this <see cref="PropertyBinding{TIn, TOut}"/> should continue to call <see cref="GetPropertyFunc"/> and <see cref="SetPropertyAction"/> if the <see cref="ParentObject"/> is known to currently represent a 'null' value.</param>
        public PropertyBinding(IBinding<TIn> parentObject, Func<TIn, TOut> getProperty, Action<TIn, TOut> setProperty, string propertyName = null, bool nullAllowed = false)
        {
            ParentObject = parentObject;
            GetPropertyFunc = getProperty;
            SetPropertyAction = setProperty;
            PropertyName = propertyName;
            NullAllowed = nullAllowed;

            ParentObject.CurrentValueChanged += ParentValueChanged;

            //// Setup the parent's PropertyChanged handler.
            if (ParentObject.CurrentValue != null && ParentObject.CurrentValue is INotifyPropertyChanged notifyParent)
            {
                notifyParent.PropertyChanged += ParentPropertyChanged;
            }
            oldParent = ParentObject.CurrentValue;

            UpdateBinding();
        }


        private TIn oldParent;
        private void ParentValueChanged(object sender, EventArgs e)
        {
            //// Replace PropertyChanged event handlers and call UpdateBinding().
            if (oldParent != null && oldParent is INotifyPropertyChanged notifyOldParent)
            {
                notifyOldParent.PropertyChanged -= ParentPropertyChanged;
            }
            UpdateBinding();
            if(ParentObject.CurrentValue != null && ParentObject.CurrentValue is INotifyPropertyChanged notifyNewParent)
            {
                notifyNewParent.PropertyChanged += ParentPropertyChanged;
            }
            oldParent = ParentObject.CurrentValue;
        }

        private void ParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //// Only cal UpdateBinding() if this change is of the property we care about.
            if (PropertyName == null || PropertyName == e.PropertyName)
            {
                UpdateBinding();
            }
        }

        /// <inheritdoc/>
        protected override TOut GetValue()
        {
            if(NullAllowed || ParentObject.CurrentValue != null)
            {
                return GetPropertyFunc(ParentObject.CurrentValue);
            }
            else
            {
                return default(TOut);
            }
        }

        /// <inheritdoc/>
        protected override void SetValue(TOut newVal)
        {
            if (NullAllowed || ParentObject.CurrentValue != null)
            {
                SetPropertyAction(ParentObject.CurrentValue, newVal);
            }
        }
    }
}