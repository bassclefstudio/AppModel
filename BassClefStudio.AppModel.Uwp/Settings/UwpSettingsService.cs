using System.Collections.Generic;
using Windows.Storage;

namespace BassClefStudio.AppModel.Settings.Uwp
{
    public class UwpSettingsService : ISettingsService
    {
        private ApplicationDataContainer SettingsContainer { get; }

        public UwpSettingsService()
        {
            SettingsContainer = ApplicationData.Current.LocalSettings;
        }

        public string this[string key] 
        { 
            get => SettingsContainer.Values[key].ToString();
            set => SettingsContainer.Values[key] = value;
        }

        public IEnumerable<string> GetKeys()
        {
            return SettingsContainer.Values.Keys;
        }
    }
}
