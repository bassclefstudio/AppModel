using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace BassClefStudio.AppModel.Settings
{
    /// <summary>
    /// An <see cref="ISettingsService"/> which uses the UWP native settings API.
    /// </summary>
    public class UwpSettingsService : ISettingsService
    {
        internal ApplicationDataContainer SettingsContainer { get; }
        /// <summary>
        /// Creates a new <see cref="UwpSettingsService"/>.
        /// </summary>
        public UwpSettingsService()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings;
        }

        /// <inheritdoc/>
        public async Task<bool> ContainsKeyAsync(string key)
        {
            return SettingsContainer.Values.ContainsKey(key);
        }

        /// <inheritdoc/>
        public async Task<T> GetValueAsync<T>(string key)
        {
            var json = SettingsContainer.Values[key].ToString();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string key, object value)
        {
            var json = JsonConvert.SerializeObject(value); 
            SettingsContainer.Values[key] = json;
        }
    }
}
