using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a Blazor <see cref="IView"/> declaration, which is linked to a URL for a component UI as well as the given AppModel <see cref="IViewModel"/>.
    /// </summary>
    public abstract class BlazorView : IView
    {
        /// <summary>
        /// The URL path for the router to the UI for this <see cref="BlazorView"/>.
        /// </summary>
        public abstract string ViewPath { get; }

        /// <inheritdoc/>
        public void Initialize()
        { }
    }

    /// <summary>
    /// A <see cref="BlazorView"/> with a strongly-typed <see cref="IViewModel"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IViewModel"/> for this view.</typeparam>
    public abstract class BlazorView<T> : BlazorView, IView<T> where T : IViewModel
    {
        /// <inheritdoc/>
        public T ViewModel { get; set; }
    }
}
