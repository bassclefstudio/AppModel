using BassClefStudio.AppModel.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a view-model, providing data for a view, in the MVVM framework.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Called asynchnously after all dependencies that this <see cref="IViewModel"/> requires have been resolved through dependency injection.
        /// </summary>
        /// <param name="parameter">A parameter passed to the view-model when this <see cref="IViewModel"/> is navigated to.</param>
        Task InitializeAsync(object parameter = null);
    }
}
