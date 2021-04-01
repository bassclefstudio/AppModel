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
    public class BlazorNavigationService : NavigationService<BlazorView>, INavigationService
    {
        private IBlazorViewProvider ViewProvider { get; }
        private NavigationManager NavigationManager { get; }
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
        public override void InitializeNavigation()
        { }

        /// <inheritdoc/>
        protected override bool NavigateInternal(BlazorView view)
        {
            Console.WriteLine($"Navigate {view.ViewPath}.");
            ViewProvider.CurrentView = view;
            NavigationManager.NavigateTo($"{NavigationManager.BaseUri}{view.ViewPath}");
            return true;
        }
    }
}
