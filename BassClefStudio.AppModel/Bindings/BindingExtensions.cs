using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// Provides extension methods for the <see cref="IBinding{T}"/> interface.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Creates a <see cref="PropertyBinding{TIn, TOut}"/> stack to bind from some given <typeparamref name="T1"/> object, through a property path, to a given <typeparamref name="T2"/> property, using reflection.
        /// </summary>
        /// <typeparam name="T1">The type of the <paramref name="baseObject"/>.</typeparam>
        /// <typeparam name="T2">The desired type of the property.</typeparam>
        /// <param name="baseObject">The <typeparamref name="T1"/> object which serves as the base of the binding with a <see cref="ConstantBinding{T}"/> expression.</param>
        /// <param name="propertyPath">The <see cref="string"/> path from the <typeparamref name="T1"/> object to the <typeparamref name="T2"/> property.</param>
        /// <returns>An <see cref="IBinding{T}"/> that references a <typeparamref name="T2"/> object.</returns>
        public static IBinding<T2> CreateReflectionBinding<T1,T2>(this T1 baseObject, string propertyPath)
        {
            var pathParts = propertyPath.Split(new string[] { "." }, StringSplitOptions.None);
            Type currentType = baseObject.GetType();
            IBinding<object> currentBinding = new ConstantBinding<object>(baseObject);
            foreach (var part in pathParts)
            {
                var property = currentType.GetProperty(part);
                currentBinding = currentBinding.WithProperty(o => property.GetValue(o), (o, val) => property.SetValue(o, val), property.Name);
            }
            return currentBinding.CastBinding<object, T2>();
        }

        /// <summary>
        /// Creates a new <see cref="PropertyBinding{TIn, TOut}"/> with the parent as the given <see cref="IBinding{T}"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the <see cref="PropertyBinding{TIn, TOut}.ParentObject"/>.</typeparam>
        /// <typeparam name="T2">The type of the resulting <see cref="IBinding{T}.StoredValue"/>.</typeparam>
        /// <param name="binding">An <see cref="IBinding{T}"/> to the parent object from which the property is retrieved.</param>
        /// <param name="getProperty">A function that gets the <typeparamref name="T2"/> property from a <typeparamref name="T1"/> parent.</param>
        /// <param name="setProperty">A method that sets the <typeparamref name="T2"/> property of a <typeparamref name="T1"/> parent.</param>
        /// <param name="propertyName">The name of the <typeparamref name="T2"/> property, which can be used to selectively trigger <see cref="Binding{T}.UpdateBinding"/> only when a property of this name is changed on the <see cref="PropertyBinding{TIn, TOut}.ParentObject"/>.</param>
        /// <param name="nullAllowed">A <see cref="bool"/> indicaT1g whether this <see cref="PropertyBinding{T1, T2}"/> should conT1ue to call <see cref="PropertyBinding{TIn, TOut}.GetPropertyFunc"/> and <see cref="PropertyBinding{TIn, TOut}.SetPropertyAction"/> if the <see cref="PropertyBinding{TIn, TOut}.ParentObject"/> is known to currently represent a 'null' value.</param>
        /// <returns></returns>
        public static IBinding<T2> WithProperty<T1, T2>(this IBinding<T1> binding, Func<T1, T2> getProperty, Action<T1, T2> setProperty, string propertyName = null, bool nullAllowed = false)
        {
            return new PropertyBinding<T1, T2>(binding, getProperty, setProperty, propertyName, nullAllowed);
        }

        /// <summary>
        /// Creates a new one-way <see cref="PropertyBinding{TIn, TOut}"/> with the parent as the given <see cref="IBinding{T}"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the <see cref="PropertyBinding{TIn, TOut}.ParentObject"/>.</typeparam>
        /// <typeparam name="T2">The type of the resulting <see cref="IBinding{T}.StoredValue"/>.</typeparam>
        /// <param name="binding">An <see cref="IBinding{T}"/> to the parent object from which the property is retrieved.</param>
        /// <param name="getProperty">A function that gets the <typeparamref name="T2"/> property from a <typeparamref name="T1"/> parent.</param>
        /// <param name="propertyName">The name of the <typeparamref name="T2"/> property, which can be used to selectively trigger <see cref="Binding{T}.UpdateBinding"/> only when a property of this name is changed on the <see cref="PropertyBinding{TIn, TOut}.ParentObject"/>.</param>
        /// <param name="nullAllowed">A <see cref="bool"/> indicaT1g whether this <see cref="PropertyBinding{T1, T2}"/> should conT1ue to call <see cref="PropertyBinding{TIn, TOut}.GetPropertyFunc"/> and <see cref="PropertyBinding{TIn, TOut}.SetPropertyAction"/> if the <see cref="PropertyBinding{TIn, TOut}.ParentObject"/> is known to currently represent a 'null' value.</param>
        /// <returns></returns>
        public static IBinding<T2> WithProperty<T1, T2>(this IBinding<T1> binding, Func<T1, T2> getProperty, string propertyName = null, bool nullAllowed = false)
        {
            return new PropertyBinding<T1, T2>(binding, getProperty, propertyName, nullAllowed);
        }

        /// <summary>
        /// Creates a <see cref="TransformBinding{TIn, TOut}"/> that casts <see cref="IBinding{T}.StoredValue"/>s to and from different types.
        /// </summary>
        /// <typeparam name="T1">The initial type of the <see cref="IBinding{T}"/>.</typeparam>
        /// <typeparam name="T2">The desired (cast) type of the <see cref="IBinding{T}"/>.</typeparam>
        /// <param name="binding">The initial <see cref="IBinding{T}"/> expression.</param>
        public static IBinding<T2> CastBinding<T1, T2>(this IBinding<T1> binding)
        {
            return new TransformBinding<T1, T2>(
                binding,
                t1 => t1 is T2 t2
                        ? t2
                        : throw new BindingException($"Invalid casting in TransformBinding: {t1?.GetType()?.Name} to {typeof(T2).Name}."),
                t2 => t2 is T1 t1
                        ? t1
                        : throw new BindingException($"Invalid casting in TransformBinding: {t2?.GetType()?.Name} to {typeof(T1).Name}."));
        }
    }
}
