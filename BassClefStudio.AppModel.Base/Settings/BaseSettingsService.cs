using BassClefStudio.AppModel.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Settings
{
    /// <summary>
    /// An <see cref="ISettingsService"/> implementation that saves settings to the <see cref="IStorageService.AppDataFolder"/> as JSON files.
    /// </summary>
    public class BaseSettingsService : ISettingsService
    {
        internal IStorageService StorageService { get; }
        private JObject Settings { get; set; }
        /// <summary>
        /// Creates a <see cref="BaseSettingsService"/> for managing settings.
        /// </summary>
        /// <param name="storageService">The <see cref="IStorageService"/> to save and manage settings files.</param>
        public BaseSettingsService(IStorageService storageService)
        {
            StorageService = storageService;
        }

        private bool Initialized { get; set; }
        private async Task ReadSettings(bool refresh = false)
        {
            if(!Initialized || refresh)
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync("Settings.json", CollisionOptions.OpenExisting);
                Settings = JObject.Parse(await file.ReadTextAsync());
                Initialized = true;
            }
        }

        private async Task WriteSettings()
        {
            if (Settings != null)
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync("Settings.json", CollisionOptions.OpenExisting);
                var json = new JObject(Settings);
                await file.WriteTextAsync(json.ToString());
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            await ReadSettings();
            return Settings.ContainsKey(key);
        }

        /// <inheritdoc/>
        public async Task<T> GetValueAsync<T>(string key)
        {
            await ReadSettings();
            return Settings[key].ToObject<T>();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string key, object value)
        {
            await ReadSettings();
            Settings[key] = JToken.FromObject(value);
            await WriteSettings();
        }
    }
}
