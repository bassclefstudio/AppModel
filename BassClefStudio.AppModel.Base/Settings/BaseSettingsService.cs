using BassClefStudio.AppModel.Storage;
using Newtonsoft.Json;
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
        private IStorageService StorageService { get; }
        private List<BaseSetting> Settings { get; }

        /// <summary>
        /// Creates a <see cref="BaseSettingsService"/> for managing settings.
        /// </summary>
        /// <param name="storageService">The <see cref="IStorageService"/> to save and manage settings files.</param>
        public BaseSettingsService(IStorageService storageService)
        {
            StorageService = storageService;
            Settings = new List<BaseSetting>();
        }

        private async Task ReadSettings(bool refresh = false)
        {
            if(Settings == null || refresh)
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync("Settings.json", CollisionOptions.OpenExisting);
                var json = JsonConvert.DeserializeObject<BaseSetting[]>(await file.ReadTextAsync());
                if(json != null)
                {
                    Settings.Clear();
                    Settings.AddRange(json);
                }
            }
        }

        private async Task WriteSettings()
        {
            if (Settings != null)
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync("Settings.json", CollisionOptions.OpenExisting);
                await file.WriteTextAsync(JsonConvert.SerializeObject(Settings));
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetKeys()
        {
            await ReadSettings();
            return Settings.Select(s => s.Key);
        }

        /// <inheritdoc/>
        public async Task<string> GetValue(string key)
        {
            await ReadSettings();
            return Settings.First(s => s.Key == key).Value;
        }

        /// <inheritdoc/>
        public async Task SetValue(string key, string value)
        {
            await ReadSettings();
            var setting = Settings.FirstOrDefault(s => s.Key == key);
            if (setting == null)
            {
                setting = new BaseSetting(key, value);
                Settings.Add(setting);
            }
            else
            {
                setting.Value = value;
            }
            await WriteSettings();
        }
    }

    internal class BaseSetting
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public BaseSetting() { }
        public BaseSetting(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
