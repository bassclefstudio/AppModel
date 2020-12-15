using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Settings
{
    /// <summary>
    /// A service that provides a storage location for key/value based local settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Checks whether the <see cref="ISettingsService"/> contains a setting of the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The desired <see cref="string"/> key.</param>
        Task<bool> ContainsKeyAsync(string key);

        /// <summary>
        /// Gets the <typeparamref name="T"/> value at the given <see cref="string"/> key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <typeparam name="T">The type of value to return.</typeparam>
        Task<T> GetValueAsync<T>(string key);

        /// <summary>
        /// Sets the <see cref="string"/> value at the given <see cref="string"/> key.
        /// </summary>
        /// <param name="key">The <see cref="string"/> key where the setting is found.</param>
        /// <param name="value">The desired <see cref="object"/> value to store in the keyed location.</param>
        Task SetValueAsync(string key, object value);
    }
}
