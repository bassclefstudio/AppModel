using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a view in the MVVM framework.
    /// </summary>
    public interface IView
    { }

    /// <summary>
    /// Represents a view with an attached <see cref="IViewModel"/> in the MVVM framework.
    /// </summary>
    /// <typeparam name="T">The type of view-model this view requires.</typeparam>
    public interface IView<T> : IView where T : IViewModel
    {
        /// <summary>
        /// The <typeparamref name="T"/> view-model for this <see cref="IView{T}"/>.
        /// </summary>
        T ViewModel { get; set; }
    }
}
