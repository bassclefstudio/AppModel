using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// Represents an <see cref="IStream{T}"/>/<see cref="SourceStream{T}"/> that is bound to the value of some observable <typeparamref name="T2"/> property.
    /// </summary>
    /// <typeparam name="T1">The type of the <see cref="INotifyPropertyChanged"/> objects that will alert the <see cref="IStream{T}"/> to incoming changes.</typeparam>
    /// <typeparam name="T2">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
    public class PropertyStream<T1, T2> : IStream<T2> where T1 : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event EventHandler<StreamValue<T2>> ValueEmitted;

        /// <summary>
        /// The parent <see cref="IStream{T}"/> that produces <see cref="INotifyPropertyChanged"/> objects.
        /// </summary>
        public IStream<T1> ParentStream { get; }

        private T1 currentParent;
        /// <summary>
        /// Gets the current <typeparamref name="T1"/> parent.
        /// </summary>
        protected T1 CurrentParent
        {
            get => currentParent;
            set
            {
                if (currentParent != null)
                {
                    currentParent.PropertyChanged -= ParentPropertyChanged;
                }
                currentParent = value;
                if (currentParent != null)
                {
                    currentParent.PropertyChanged += ParentPropertyChanged;
                    CurrentValue = GetProperty(currentParent);
                }
            }
        }

        private T2 currentValue;
        /// <summary>
        /// Represents the currently stored <typeparamref name="T2"/> property value.
        /// </summary>
        protected T2 CurrentValue
        {
            get => currentValue;
            set
            {
                if (!currentValue.Equals(value))
                {
                    currentValue = value;
                    ValueEmitted?.Invoke(this, new StreamValue<T2>(currentValue));
                }
            }
        }

        /// <summary>
        /// The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> property from the <typeparamref name="T1"/> parent.
        /// </summary>
        public Func<T1, T2> GetProperty { get; set; }

        /// <summary>
        /// Creates a new <see cref="PropertyStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> that produces <see cref="INotifyPropertyChanged"/> objects.</param>
        /// <param name="getProperty">The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> property from the <typeparamref name="T1"/> parent.</param>
        public PropertyStream(IStream<T1> parent, Func<T1, T2> getProperty)
        {
            ParentStream = parent;
            GetProperty = getProperty;
            ParentStream.ValueEmitted += ParentValueEmitted;
        }

        /// <inheritdoc/>
        public async Task StartAsync()
        {
            await ParentStream.StartAsync();
        }

        private void ParentValueEmitted(object sender, StreamValue<T1> e)
        {
            if (e.DataType == StreamValueType.Result)
            {
                CurrentParent = e.Result;
            }
            else if(e.DataType == StreamValueType.Error)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>(e.Error));
            }
        }

        private void ParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CurrentValue = GetProperty(CurrentParent);
        }
    }
}
