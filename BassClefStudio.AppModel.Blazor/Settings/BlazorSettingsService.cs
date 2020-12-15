using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Settings
{
    /// <summary>
    /// An <see cref="ISettingsService"/> which uses localStorage on the web to store settings.
    /// </summary>
    public class BlazorSettingsService : ISettingsService
    {
        internal ILocalStorageService LocalStorageService { get; }
        /// <summary>
        /// Creates a new <see cref="BlazorSettingsService"/>.
        /// </summary>
        /// <param name="localStorageService">The Blazor localStorage <see cref="ILocalStorageService"/>.</param>
        public BlazorSettingsService(ILocalStorageService localStorageService)
        {
            LocalStorageService = localStorageService;
        }

        /// <inheritdoc/>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return await LocalStorageService.ContainKeyAsync(key);
        }

        /// <inheritdoc/>
        public async Task<T> GetValueAsync<T>(string key)
        {
            return await LocalStorageService.GetItemAsync<T>(key);
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string key, object value)
        {
            await LocalStorageService.SetItemAsync(key, value);
        }
    }
}
