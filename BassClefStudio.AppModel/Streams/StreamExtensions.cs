using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Streams
{
    /// <summary>
    /// Provides a series of extension methods for dealing with <see cref="IStream{T}"/>s of all types.
    /// </summary>
    public static class StreamExtensions
    {
        #region Cast

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the values of the given <see cref="IStream{T}"/> as type <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the cast values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns cast <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> As<T1, T2>(this IStream<T1> stream) where T1 : T2
        {
            return new MapStream<T1, T2>(stream, t1 => (T2)t1);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that attempts to cast the values of the given <see cref="IStream{T}"/> as type <typeparamref name="T2"/>.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the cast values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns cast <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Cast<T1, T2>(this IStream<T1> stream)
        {
            return new MapStream<T1, T2>(
                stream,
                t1 => t1 is T2 t2
                    ? t2
                    : throw new StreamException($"Invalid casting in MapStream: {t1?.GetType()?.Name} to {typeof(T2).Name}."));
        }

        #endregion
        #region Select

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that maps the values of the given <see cref="IStream{T}"/> as <typeparamref name="T2"/> values.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Select<T1, T2>(this IStream<T1> stream, Func<T1, T2> mapFunc)
        {
            return new MapStream<T1, T2>(stream, mapFunc);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that maps the values of the given <see cref="IStream{T}"/> as <typeparamref name="T2"/> values.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Select<T1, T2>(this IStream<T1> stream, Func<T1, Task<T2>> mapFunc)
        {
            return new MapStream<T1, T2>(stream, mapFunc);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that maps the values of the given <see cref="IStream{T}"/> as <typeparamref name="T2"/> values in parallel.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="mapFunc">The asynchronous function for converting each <typeparamref name="T1"/> item to its <typeparamref name="T2"/> representation.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values as they're converted.</returns>
        public static IStream<T2> SelectParallel<T1, T2>(this IStream<T1> stream, Func<T1, Task<T2>> mapFunc)
        {
            return new ParallelMapStream<T1, T2>(stream, mapFunc);
        }

        #endregion
        #region Where

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that filters the values returned by the given <see cref="IStream{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T"/> values.</param>
        /// <param name="filter">A function that returns a <see cref="bool"/> for each <typeparamref name="T"/> input indicating whether it should propogate onto this stream.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns only the filtered <typeparamref name="T"/> values.</returns>
        public static IStream<T> Where<T>(this IStream<T> stream, Func<T, bool> filter)
        {
            return new FilterStream<T>(stream, filter);
        }

        #endregion
        #region Aggregate

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that aggregates the values of the given <see cref="IStream{T}"/> into a <typeparamref name="T2"/> returned state.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The asynchronous function that returns a new <typeparamref name="T2"/> aggregate from the current <typeparamref name="T2"/> state and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> state.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Aggregate<T1, T2>(this IStream<T1> stream, Func<T2, T1, Task<T2>> aggregateFunc, T2 initialState = default(T2))
        {
            return new AggregateStream<T1, T2>(stream, aggregateFunc);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that aggregates the values of the given <see cref="IStream{T}"/> into a <typeparamref name="T2"/> returned state.
        /// </summary>
        /// <typeparam name="T1">The type of values returned by the parent <see cref="IStream{T}"/>.</typeparam>
        /// <typeparam name="T2">The type of the values this <see cref="IStream{T}"/> returns.</typeparam>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <typeparamref name="T1"/> values.</param>
        /// <param name="aggregateFunc">The function that returns a new <typeparamref name="T2"/> aggregate from the current <typeparamref name="T2"/> state and the next <typeparamref name="T1"/> input.</param>
        /// <param name="initialState">The initial <typeparamref name="T2"/> state.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting <typeparamref name="T2"/> values.</returns>
        public static IStream<T2> Aggregate<T1, T2>(this IStream<T1> stream, Func<T2, T1, T2> aggregateFunc, T2 initialState = default(T2))
        {
            return new AggregateStream<T1, T2>(stream, aggregateFunc);
        }

        #endregion
        #region Sum

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the sum of all the values returned by the given <see cref="IStream{T}"/> up to that point.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="int"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting sums.</returns>
        public static IStream<int> Sum(this IStream<int> stream)
        {
            return new AggregateStream<int, int>(stream, (sum, val) => sum + val, 0);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the sum of all the values returned by the given <see cref="IStream{T}"/> up to that point.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="double"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting sums.</returns>
        public static IStream<double> Sum(this IStream<double> stream)
        {
            return new AggregateStream<double, double>(stream, (sum, val) => sum + val, 0);
        }

        /// <summary>
        /// Creates an <see cref="IStream{T}"/> that returns the sum of all the values returned by the given <see cref="IStream{T}"/> up to that point.
        /// </summary>
        /// <param name="stream">The parent <see cref="IStream{T}"/> producing <see cref="int"/> values.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns resulting sums.</returns>
        public static IStream<float> Sum(this IStream<float> stream)
        {
            return new AggregateStream<float, float>(stream, (sum, val) => sum + val, 0);
        }

        #endregion
        #region Bind

        /// <summary>
        /// Binds the incoming <typeparamref name="T"/> results from an <see cref="IStream{T}"/> to a given <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that takes in an input <typeparamref name="T"/> value and will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Result"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static IStream<T> BindResult<T>(this IStream<T> stream, Action<T> action)
        {
            stream.ValueEmitted += (s, e) =>
            {
                if(e.DataType == StreamValueType.Result)
                {
                    action(e.Result);
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds any incoming <see cref="Exception"/>s from an <see cref="IStream{T}"/> to a given <see cref="Action{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that takes in an input <see cref="Exception"/> and will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Error"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static IStream<T> BindError<T>(this IStream<T> stream, Action<Exception> action)
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Error)
                {
                    action(e.Error);
                }
            };
            return stream;
        }

        /// <summary>
        /// Binds the completion of an <see cref="IStream{T}"/> to a given <see cref="Action"/>.
        /// </summary>
        /// <typeparam name="T">The type of values emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="stream">The <see cref="IStream{T}"/> stream to bind to.</param>
        /// <param name="action">An action that will be executed every time the <paramref name="stream"/> emits a value of <see cref="StreamValueType.Completed"/>.</param>
        /// <returns>The input <see cref="IStream{T}"/> <paramref name="stream"/>.</returns>
        public static IStream<T> BindComplete<T>(this IStream<T> stream, Action action)
        {
            stream.ValueEmitted += (s, e) =>
            {
                if (e.DataType == StreamValueType.Completed)
                {
                    action();
                }
            };
            return stream;
        }

        #endregion
        #region Properties

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that emits the change-notified values of the given <typeparamref name="T2"/> property on this <typeparamref name="T1"/> object.
        /// </summary>
        /// <typeparam name="T1">The type of the (usually <see cref="INotifyPropertyChanged"/>) objects that will alert the <see cref="IStream{T}"/> to incoming changes.</typeparam>
        /// <typeparam name="T2">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
        /// <param name="parent">The parent <see cref="IStream{T}"/> that produces parent objects.</param>
        /// <param name="getProperty">The <see cref="Func{T, TResult}"/> that gets the <typeparamref name="T2"/> property from the <typeparamref name="T1"/> parent.</param>
        /// <param name="propertyName">For debugging purposes, include the name of the property this <see cref="IStream{T}"/> is retrieving.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns the <typeparamref name="T2"/> property values.</returns>
        public static IStream<T2> Property<T1, T2>(this IStream<T1> parent, Func<T1, T2> getProperty, string propertyName = null)
        {
            return new PropertyStream<T1, T2>(parent, getProperty, propertyName);
        }

        /// <summary>
        /// Returns an <see cref="IStream{T}"/> that emits the change-notified values of the given <typeparamref name="T2"/> property on this <typeparamref name="T1"/> object.
        /// </summary>
        /// <typeparam name="T1">The type of the (usually <see cref="INotifyPropertyChanged"/>) objects that will alert the <see cref="IStream{T}"/> to incoming changes.</typeparam>
        /// <typeparam name="T2">The type of output values this <see cref="IStream{T}"/> produces.</typeparam>
        /// <param name="parent">The parent <see cref="IStream{T}"/> that produces parent objects.</param>
        /// <param name="propertyPath">The <see cref="string"/>, dot-delimited path to the desired property.</param>
        /// <returns>An <see cref="IStream{T}"/> that returns <typeparamref name="T2"/> property values through reflection.</returns>
        public static IStream<T2> Property<T1, T2>(this IStream<T1> parent, string propertyPath)
        {
            var pathParts = propertyPath.Split(new string[] { "." }, StringSplitOptions.None);
            Type currentType = typeof(T1);
            IStream<object> currentBinding = parent.Cast<T1, object>();
            foreach (var part in pathParts)
            {
                var property = currentType.GetProperty(part);
                if (property == null)
                {
                    throw new StreamException($"Failed to find property with name {part} on type {currentType.Name}.");
                }

                currentBinding = currentBinding.Property(
                         o => property.GetValue(o),
                         property.Name);

                currentType = property.PropertyType;
            }
            return currentBinding.Cast<object, T2>();
        }

        #endregion
        #region Source

        /// <summary>
        /// Returns a deferred <see cref="IStream{T}"/> that will emit the given value when <see cref="IStream{T}.StartAsync"/> is called.
        /// </summary>
        /// <typeparam name="T">The type of value emitted by this <see cref="IStream{T}"/>.</typeparam>
        /// <param name="value">The singular <typeparamref name="T"/> value to emit.</param>
        /// <returns>An <see cref="IStream{T}"/> that will emit <paramref name="value"/> when <see cref="IStream{T}.StartAsync"/> is called.</returns>
        public static IStream<T> AsStream<T>(this T value)
        {
            return new SourceStream<T>(value);
        }

        #endregion
    }
}
