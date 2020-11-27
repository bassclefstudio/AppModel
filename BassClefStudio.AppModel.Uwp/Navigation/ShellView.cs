using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// A <see cref="Page"/> and <see cref="IView{T}"/> that can provide navigation chrome and a <see cref="Frame"/> for continued navigation.
    /// </summary>
    /// <typeparam name="T">The type of the view-model <see cref="IShellHandler"/> this page uses.</typeparam>
    public abstract class ShellPage<T> : Page, IView<T> where T : IShellHandler
    {
        /// <inheritdoc/>
        public T ViewModel { get; set; }

        /// <summary>
        /// The <see cref="Frame"/> that the <see cref="UwpNavigationService"/> should use for navigation inside this <see cref="ShellPage{T}"/>'s 'chrome'.
        /// </summary>
        public abstract Frame NavFrame { get; set; }

        internal INavigationService NavService { get; }
        public ShellPage(INavigationService navService)
        {
            NavService = navService;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            if(NavService is UwpNavigationService uwpNavService)
            {
                uwpNavService.CurrentFrame = this.NavFrame;
            }
        }
    }
}
