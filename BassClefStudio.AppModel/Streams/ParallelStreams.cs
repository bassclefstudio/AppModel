using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Streams
{
    /// <summary>
    /// Represents a basic <see cref="IStream{T}"/> that transforms the returned values from another parent <see cref="IStream{T}"/> asynchronously. The resulting values are added to the stream in the order they're completed.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public abstract class ParallelStream<T1, T2> : IStream<T2>
    {
        /// <inheritdoc/>
        public event EventHandler<StreamValue<T2>> ValueEmitted;

        /// <summary>
        /// The parent <see cref="IStream{T}"/> this <see cref="ParallelStream{T1, T2}"/> is based on.
        /// </summary>
        public IStream<T1> ParentStream { get; }

        /// <summary>
        /// An asynchronous task that takes a <typeparamref name="T1"/> object and produces a <typeparamref name="T2"/> object.
        /// </summary>
        /// <param name="input">The input <typeparamref name="T1"/> from <see cref="ParentStream"/>.</param>
        protected abstract Task<T2> ProduceValue(T1 input);

        /// <summary>
        /// Creates a new <see cref="ParallelStream{T1, T2}"/> from the 
        /// </summary>
        /// <param name="parent"></param>
        public ParallelStream(IStream<T1> parent)
        {
            ParentStream = parent;
            ParentStream.ValueEmitted += ParentValueEmitted;
        }

        /// <inheritdoc/>
        public async Task StartAsync()
        {
            await ParentStream.StartAsync();
        }

        private void ParentValueEmitted(object sender, StreamValue<T1> e)
        {
            _ = ProcessInput(e);
        }

        private async Task ProcessInput(StreamValue<T1> current)
        {
            if (current.DataType == StreamValueType.Completed)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>());
            }
            else if (current.DataType == StreamValueType.Error)
            {
                ValueEmitted?.Invoke(this, new StreamValue<T2>(current.Error));
            }
            else if (current.DataType == StreamValueType.Result)
            {
                try
                {
                    var output = await ProduceValue(current.Result);
                    ValueEmitted?.Invoke(this, new StreamValue<T2>(output));
                }
                catch (Exception ex)
                {
                    ValueEmitted?.Invoke(this, new StreamValue<T2>(ex));
                }
            }
        }
    }

    /// <summary>
    /// An <see cref="IStream{T}"/>/<see cref="ParallelStream{T1, T2}"/> that maps each <typeparamref name="T1"/> value from the parent stream into a <typeparamref name="T2"/> value asynchronously and in parallel. The resulting values are added to the stream in the order they're completed.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class ParallelMapStream<T1, T2> : ParallelStream<T1, T2>
    {
        /// <summary>
        /// The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.
        /// </summary>
        public Func<T1, Task<T2>> ProduceFunc { get; }

        /// <summary>
        /// Creates a new <see cref="ParallelMapStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        public ParallelMapStream(IStream<T1> parent, Func<T1, Task<T2>> mapFunc) : base(parent)
        {
            ProduceFunc = mapFunc;
        }

        /// <inheritdoc/>
        protected override async Task<T2> ProduceValue(T1 input)
        {
            return await ProduceFunc(input);
        }
    }
}
