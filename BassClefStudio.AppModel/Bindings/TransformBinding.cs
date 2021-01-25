using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Bindings
{
    /// <summary>
    /// An <see cref="IBinding{T}"/> expression that can convert to and from two types of objects, backed by an existing <see cref="IBinding{T}"/>.
    /// </summary>
    /// <typeparam name="TIn">The type of the <see cref="InitialBinding"/>.</typeparam>
    /// <typeparam name="TOut">The type of the <see cref="IBinding{T}.CurrentValue"/>.</typeparam>
    public class TransformBinding<TIn, TOut> : Binding<TOut>
    {
        /// <summary>
        /// The initial <see cref="IBinding{T}"/> expression to a <typeparamref name="TIn"/> value which this <see cref="TransformBinding{TIn, TOut}"/> uses.
        /// </summary>
        public IBinding<TIn> InitialBinding { get; }

        /// <summary>
        /// Creates a new <see cref="TransformBinding{TIn, TOut}"/> from a given <see cref="InitialBinding"/>.
        /// </summary>
        /// <param name="initialBinding">The initial <see cref="IBinding{T}"/> expression to a <typeparamref name="TIn"/> value which this <see cref="TransformBinding{TIn, TOut}"/> uses.</param>
        /// <param name="getFunc">A function that creates a <typeparamref name="TOut"/> from a <typeparamref name="TIn"/>.</param>
        /// <param name="setFunc">A function that creates a <typeparamref name="TIn"/> from a <typeparamref name="TOut"/>.</param>
        public TransformBinding(IBinding<TIn> initialBinding, Func<TIn, TOut> getFunc, Func<TOut, TIn> setFunc)
        {
            InitialBinding = initialBinding;
            GetFunc = getFunc;
            SetFunc = setFunc;
            InitialBinding.CurrentValueChanged += InitialValueChanged;
            UpdateBinding();
        }

        private void InitialValueChanged(object sender, EventArgs e) => UpdateBinding();

        /// <summary>
        /// A function that creates a <typeparamref name="TOut"/> from a <typeparamref name="TIn"/>.
        /// </summary>
        public Func<TIn, TOut> GetFunc { get; set; }

        /// <inheritdoc/>
        protected override TOut GetValue() => GetFunc(InitialBinding.CurrentValue);

        /// <summary>
        /// A function that creates a <typeparamref name="TIn"/> from a <typeparamref name="TOut"/>.
        /// </summary>
        public Func<TOut, TIn> SetFunc { get; set; }

        /// <inheritdoc/>
        protected override void SetValue(TOut newVal) => InitialBinding.CurrentValue = SetFunc(newVal);
    }
}
