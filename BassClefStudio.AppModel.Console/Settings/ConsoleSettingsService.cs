using BassClefStudio.AppModel.Storage;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Settings
{
    public class ConsoleSettingsService : ISettingsService
    {
        private IStorageService StorageService { get; }
        private List<ConsoleSetting> Settings { get; set; }

        public ConsoleSettingsService(IStorageService storageService)
        {
            StorageService = storageService;
        }

        private async Task ReadSettings(bool refresh = false)
        {
            if(Settings == null || refresh)
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync("Settings.json", CollisionOptions.OpenExisting);
                using (var settingsContent = await file.OpenFileAsync(FileOpenMode.Read))
                {
                    Settings = JsonConvert.DeserializeObject<ConsoleSetting[]>(await settingsContent.ReadTextAsync()).ToList();
                }
            }
        }

        private async Task WriteSettings()
        {
            if (Settings != null)
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync("Settings.json", CollisionOptions.OpenExisting);
                using (var settingsContent = await file.OpenFileAsync(FileOpenMode.ReadWrite))
                {
                    await settingsContent.WriteTextAsync(JsonConvert.SerializeObject(Settings));
                }
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
                setting = new ConsoleSetting(key, value);
                Settings.Add(setting);
            }
            else
            {
                setting.Value = value;
            }
            await WriteSettings();
        }
    }

    internal class ConsoleSetting
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ConsoleSetting() { }
        public ConsoleSetting(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
