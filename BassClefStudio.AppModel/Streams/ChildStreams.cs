﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Streams
{
    /// <summary>
    /// Represents a basic <see cref="IStream{T}"/> that transforms the returned values from another parent <see cref="IStream{T}"/>, one-to-one, in their initial order.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public abstract class ChildStream<T1, T2> : IStream<T2>
    {
        /// <inheritdoc/>
        public event EventHandler<StreamValue<T2>> ValueEmitted;

        /// <summary>
        /// The parent <see cref="IStream{T}"/> this <see cref="ChildStream{T1, T2}"/> is based on.
        /// </summary>
        public IStream<T1> ParentStream { get; }

        /// <summary>
        /// An asynchronous task that takes a <typeparamref name="T1"/> object and produces a <typeparamref name="T2"/> object.
        /// </summary>
        /// <param name="input">The input <typeparamref name="T1"/> from <see cref="ParentStream"/>.</param>
        protected abstract Task<T2> ProduceValue(T1 input);

        /// <summary>
        /// Creates a new <see cref="ChildStream{T1, T2}"/> from the 
        /// </summary>
        /// <param name="parent"></param>
        public ChildStream(IStream<T1> parent)
        {
            ParentStream = parent;
            ParentStream.ValueEmitted += ParentValueEmitted;
        }

        /// <inheritdoc/>
        public virtual async Task StartAsync()
        {
            await ParentStream.StartAsync();
        }

        private Queue<StreamValue<T1>> incomingInputs = new Queue<StreamValue<T1>>();
        private void ParentValueEmitted(object sender, StreamValue<T1> e)
        {
            incomingInputs.Enqueue(e);
            _ = ProcessInputs();
        }

        private async Task ProcessInputs()
        {
            while (incomingInputs.Any())
            {
                var current = incomingInputs.Dequeue();
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
    }

    /// <summary>
    /// An <see cref="IStream{T}"/>/<see cref="ChildStream{T1, T2}"/> that maps each <typeparamref name="T1"/> value from the parent stream into a <typeparamref name="T2"/> value via a given function.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class MapStream<T1, T2> : ChildStream<T1, T2>
    {
        /// <summary>
        /// The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.
        /// </summary>
        public Func<T1, Task<T2>> ProduceFunc { get; }

        /// <summary>
        /// Creates a new asynchronous <see cref="MapStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        public MapStream(IStream<T1> parent, Func<T1, Task<T2>> mapFunc) : base(parent)
        {
            ProduceFunc = mapFunc;
        }

        /// <summary>
        /// Creates a new <see cref="MapStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        public MapStream(IStream<T1> parent, Func<T1, T2> mapFunc) : base(parent)
        {
            ProduceFunc = t1 => Task.FromResult(mapFunc(t1));
        }

        /// <inheritdoc/>
        protected override async Task<T2> ProduceValue(T1 input)
        {
            return await ProduceFunc(input);
        }
    }

    /// <summary>
    /// An <see cref="IStream{T}"/>/<see cref="ChildStream{T1, T2}"/> that aggregates incoming <typeparamref name="T1"/> values from the parent stream into a <typeparamref name="T2"/> <see cref="CurrentState"/>.
    /// </summary>
    /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
    /// <typeparam name="T2">The type of the transformed values this <see cref="IStream{T}"/> returns.</typeparam>
    public class AggregateStream<T1, T2> : ChildStream<T1, T2>
    {
        /// <summary>
        /// A <typeparamref name="T2"/> value indicating the current aggregated state.
        /// </summary>
        public T2 CurrentState { get; private set; }

        /// <summary>
        /// The asynchronous function that returns a new <typeparamref name="T2"/> aggregate from the <see cref="CurrentState"/> and the next <typeparamref name="T1"/> input.
        /// </summary>
        public Func<T2, T1, Task<T2>> AggregateFunc { get; }

        /// <summary>
        /// Creates a new asynchronous <see cref="AggregateStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The asynchronous function that returns a new <typeparamref name="T2"/> aggregate from the <see cref="CurrentState"/> and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> aggregate to set the <see cref="CurrentState"/> to.</param>
        public AggregateStream(IStream<T1> parent, Func<T2, T1, Task<T2>> aggregateFunc, T2 initialState = default(T2)) : base(parent)
        {
            AggregateFunc = aggregateFunc;
            CurrentState = initialState;
        }

        /// <summary>
        /// Creates a new <see cref="AggregateStream{T1, T2}"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The function that returns a new <typeparamref name="T2"/> aggregate from the <see cref="CurrentState"/> and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> aggregate to set the <see cref="CurrentState"/> to.</param>
        public AggregateStream(IStream<T1> parent, Func<T2, T1, T2> aggregateFunc, T2 initialState = default(T2)) : base(parent)
        {
            AggregateFunc = (state, item) => Task.FromResult(aggregateFunc(state, item));
            CurrentState = initialState;
        }

        /// <inheritdoc/>
        protected override async Task<T2> ProduceValue(T1 input)
        {
            CurrentState = await AggregateFunc(CurrentState, input);
            return CurrentState;
        }
    }
}