using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Settings
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
        Task<IEnumerable<string>> GetKeys();

        /// <summary>
        /// Gets the <see cref="string"/> value at the given <see cref="string"/> key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        Task<string> GetValue(string key);

        /// <summary>
        /// Sets the <see cref="string"/> value at the given <see cref="string"/> key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <param name="value">The desired <see cref="string"/> value to store in the keyed location.</param>
        Task SetValue(string key, string value);
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
        public static async Task<bool> ContainsKey(this ISettingsService settings, string key)
        {
            return (await settings.GetKeys()).Contains(key);
        }

        /// <summary>
        /// Gets the value of the setting at the given key as type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the settings value, which will be deserialized from JSON.</typeparam>
        /// <param name="settings">This <see cref="ISettingsService"/> providing the settings data.</param>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <returns>The <typeparamref name="T"/> value stored in settings.</returns>
        public static async Task<T> GetValue<T>(this ISettingsService settings, string key)
        {
            return JsonConvert.DeserializeObject<T>(await settings.GetValue(key));
        }

        /// <summary>
        /// Sets the value of the setting at the given key to a serialized JSON object.
        /// </summary>
        /// <param name="settings">This <see cref="ISettingsService"/> providing the settings data.</param>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <param name="value">The value to store in this location on the <see cref="ISettingsService"/>.</param>
        public static async Task SetValue(this ISettingsService settings, string key, object value)
        {
            await settings.SetValue(key, JsonConvert.SerializeObject(value));
        }
    }
}
