using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace BassClefStudio.AppModel.Settings
{
    public class UwpSettingsService : ISettingsService
    {
        private ApplicationDataContainer SettingsContainer { get; }

        public UwpSettingsService()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetKeys()
        {
            return SettingsContainer.Values.Keys;
        }

        /// <inheritdoc/>
        public async Task<string> GetValue(string key)
        {
            return SettingsContainer.Values[key].ToString();
        }

        /// <inheritdoc/>
        public async Task SetValue(string key, string value)
        {
            SettingsContainer.Values[key] = value;
        }
    }
}
