using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BassClefStudio.Mvvm.Services.Settings
{
    /// <summary>
    /// A service that provides a storage location for key/value based local settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Gets the collection of keys of values currently contained in the settings store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="string"/> keys.</returns>
        IEnumerable<string> GetKeys();
        
        /// <summary>
        /// Gets or sets the <see cref="string"/> value at the given <see cref="string"/> key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        string this[string key] { get; set; }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="ISettingsService"/> interface.
    /// </summary>
    public static class SettingsExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="ISettingsService"/> contains a setting of the given <paramref name="key"/>.
        /// </summary>
        /// <param name="settings">This <see cref="ISettingsService"/> providing the settings data.</param>
        /// <param name="key">The desired <see cref="string"/> key.</param>
        /// <returns></returns>
        public static bool ContainsKey(this ISettingsService settings, string key)
        {
            return settings.GetKeys().Contains(key);
        }

        /// <summary>
        /// Gets the value of the setting at the given key as type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the settings value, which will be deserialized from JSON.</typeparam>
        /// <param name="settings">This <see cref="ISettingsService"/> providing the settings data.</param>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <returns>The <typeparamref name="T"/> value stored in settings.</returns>
        public static T GetValue<T>(this ISettingsService settings, string key)
        {
            return JsonConvert.DeserializeObject<T>(settings[key]);
        }

        /// <summary>
        /// Sets the value of the setting at the given key to a serialized JSON object.
        /// </summary>
        /// <param name="settings">This <see cref="ISettingsService"/> providing the settings data.</param>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <param name="value">The value to store in this location on the <see cref="ISettingsService"/>.</param>
        public static void SetValue(this ISettingsService settings, string key, object value)
        {
            settings[key] = JsonConvert.SerializeObject(value);
        }
    }
}
