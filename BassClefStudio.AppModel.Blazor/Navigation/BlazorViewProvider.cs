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
    /// Represents a <see cref="BlazorViewProvider"/> that navigates to URLs in the Blazor SPA and provides the current view information for components to retrieve their <see cref="IViewModel"/>.
    /// </summary>
    public class BlazorViewProvider : ViewProvider<BlazorView>, IViewProvider
    {
        private IBlazorViewInfo ViewProvider { get; }
        private NavigationManager NavigationManager { get; }
        /// <summary>
        /// Creates a new <see cref="BlazorViewProvider"/> from the required Blazor dependencies.
        /// </summary>
        /// <param name="navigationManager">The Blazor platform <see cref="Microsoft.AspNetCore.Components.NavigationManager"/>.</param>
        /// <param name="viewProvider">The <see cref="IBlazorViewInfo"/> that the <see cref="BlazorViewProvider"/> can inform about navigation events.</param>
        public BlazorViewProvider(NavigationManager navigationManager, IBlazorViewInfo viewProvider)
        {
            NavigationManager = navigationManager;
            ViewProvider = viewProvider;
        }

        /// <inheritdoc/>
        public override void StartUI()
        { }

        /// <inheritdoc/>
        protected override void SetViewInternal(NavigationRequest request, BlazorView view)
        {
            if (request.Properties.LayerMode == LayerBehavior.Default)
            {
                Console.WriteLine($"Navigate {view.ViewPath}.");
                ViewProvider.CurrentView = view;
                NavigationManager.NavigateTo($"{NavigationManager.BaseUri}{view.ViewPath}");
            }
            else
            {
                throw new ArgumentException($"Blazor apps currently do not have support for navigation layers.", nameof(request));
            }
        }
    }
}
