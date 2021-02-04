using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents an abstract 'view' in a Console app.
    /// </summary>
    public interface IConsoleView
    {
        /// <summary>
        /// Show the view, calling <see cref="Console"/> methods and displaying text to the user. Ensure that this <see cref="Task"/> never completes without calling either <see cref="Lifecycle.App.Suspend"/> or navigating to a different <see cref="ConsoleView{T}"/>.
        /// </summary>
        /// <param name="parameter">An <see cref="object"/> parameter that can be passed to the <see cref="IConsoleView{T}"/> on navigation.</param>
        Task ShowView(object parameter);
    }

    /// <summary>
    /// A basic abstract implementation of the <see cref="IConsoleView"/> interface, with an attached <see cref="IViewModel"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IViewModel"/> <see cref="ViewModel"/>.</typeparam>
    public abstract class ConsoleView<T> : IConsoleView, IView<T> where T : IViewModel
    {
        /// <inheritdoc/>
        public T ViewModel { get; set; }

        /// <inheritdoc/>
        public virtual void Initialize()
        { }

        /// <inheritdoc/>
        public abstract Task ShowView(object parameter);
    }
}
