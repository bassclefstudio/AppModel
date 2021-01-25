using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// A wrapper around an existing <see cref="IBinding{T}"/> that provides <see cref="INotifyCollectionChanged"/> change notifications.
    /// </summary>
    /// <typeparam name="T">he type of the <see cref="IBinding{T}.CurrentValue"/>.</typeparam>
    public class CollectionBinding<T> : Binding<T>
    {
        /// <summary>
        /// The base <see cref="IBinding{T}"/> containing the collection to monitor.
        /// </summary>
        public IBinding<T> BaseBinding { get; }

        /// <summary>
        /// Creates a new <see cref="CollectionBinding{T}"/>.
        /// </summary>
        /// <param name="baseBinding">The base <see cref="IBinding{T}"/> containing the collection to monitor.</param>
        public CollectionBinding(IBinding<T> baseBinding)
        {
            BaseBinding = baseBinding;
            BaseBinding.CurrentValueChanged += ParentValueChanged;

            //// Setup the parent's PropertyChanged handler.
            if (BaseBinding.CurrentValue != null && BaseBinding.CurrentValue is INotifyCollectionChanged notifyBase)
            {
                notifyBase.CollectionChanged += ParentCollectionChanged;
            }
            oldBase = BaseBinding.CurrentValue;

            UpdateBinding();
        }

        private T oldBase;
        private void ParentValueChanged(object sender, EventArgs e)
        {
            //// Replace PropertyChanged event handlers and call UpdateBinding().
            if (oldBase != null && oldBase is INotifyCollectionChanged notifyOldBase)
            {
                notifyOldBase.CollectionChanged -= ParentCollectionChanged;
            }
            UpdateBinding();
            if (BaseBinding.CurrentValue != null && BaseBinding.CurrentValue is INotifyCollectionChanged notifyNewBase)
            {
                notifyNewBase.CollectionChanged += ParentCollectionChanged;
            }
            oldBase = BaseBinding.CurrentValue;
        }

        private void ParentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => TriggerValueChanged();

        /// <inheritdoc/>
        protected override T GetValue()
        {
            return BaseBinding.CurrentValue;
        }

        /// <inheritdoc/>
        protected override void SetValue(T newVal)
        {
            BaseBinding.CurrentValue = newVal;
        }
    }
}
