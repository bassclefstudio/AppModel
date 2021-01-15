using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents a <see cref="BlazorNavigationService"/> that navigates to URLs in the Blazor SPA and provides the current view information for components to retrieve their <see cref="IViewModel"/>.
    /// </summary>
    public class BlazorNavigationService : INavigationService
    {
        internal IBlazorViewProvider ViewProvider { get; }
        internal NavigationManager NavigationManager { get; }
        /// <summary>
        /// Creates a new <see cref="BlazorNavigationService"/> from the required Blazor dependencies.
        /// </summary>
        /// <param name="navigationManager">The Blazor platform <see cref="Microsoft.AspNetCore.Components.NavigationManager"/>.</param>
        /// <param name="viewProvider">The <see cref="IBlazorViewProvider"/> that the <see cref="BlazorNavigationService"/> can inform about navigation events.</param>
        public BlazorNavigationService(NavigationManager navigationManager, IBlazorViewProvider viewProvider)
        {
            NavigationManager = navigationManager;
            ViewProvider = viewProvider;
        }

        /// <inheritdoc/>
        public void InitializeNavigation()
        { }

        /// <inheritdoc/>
        public void Navigate(IView view, object parameter = null)
        {
            Console.WriteLine($"Navigate {view}.");
            if(view is BlazorView blazorView)
            {
                ViewProvider.CurrentView = blazorView;
                NavigationManager.NavigateTo($"{NavigationManager.BaseUri}{blazorView.ViewPath}");
            }
            else
            {
                Debug.WriteLine("Blazor navigation expects a BlazorView IView instance.");
            }
        }
    }
}
