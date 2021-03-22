using Autofac;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Serialization;
using BassClefStudio.NET.Sync;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// An <see cref="ILink{T}"/> that provides a means for getting and storing data in the <see cref="ISettingsService"/> store.
    /// </summary>
    /// <typeparam name="T">The type of objects that this <see cref="ILink{T}"/> handles.</typeparam>
    public class SettingsLink<T> : ILink<T>
    {
        /// <summary>
        /// The <see cref="string"/> key or path to the place where the <typeparamref name="T"/> item should be stored.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// A <see cref="bool"/> indicating whether the <see cref="SettingsLink{T}"/> is configured to use an <see cref="BassClefStudio.NET.Serialization.ISerializationService"/> for serialization of the given <typeparamref name="T"/> values.
        /// </summary>
        public bool UseSerializer { get; }

        internal ISettingsService SettingsService { get; }
        internal IDispatcherService DispatcherService { get; }
        internal ISerializationService SerializationService { get; }
        /// <summary>
        /// Creates a new <see cref="SettingsLink{T}"/> from the required services.
        /// </summary>
        public SettingsLink(ISettingsService settingsService, IDispatcherService dispatcherService)
        {
            SettingsService = settingsService;
            DispatcherService = dispatcherService;
            UseSerializer = false;
        }

        /// <summary>
        /// Creates a new <see cref="SettingsLink{T}"/> from the required services and optional <see cref="ISerializationService"/>.
        /// </summary>
        public SettingsLink(ISettingsService settingsService, IDispatcherService dispatcherService, ISerializationService serializationService)
        {
            SettingsService = settingsService;
            DispatcherService = dispatcherService;
            SerializationService = serializationService;
            UseSerializer = SerializationService.IsSerializable<T>();
        }

        /// <inheritdoc/>
        public async Task PushAsync(ISyncItem<T> item)
        { 
            if (UseSerializer)
            {
                string json = SerializationService.Serialize(item.Item);
                await SettingsService.SetValueAsync(Key, json);
            }
            else
            {
                await SettingsService.SetValueAsync(Key, item.Item);
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(ISyncItem<T> item)
            => await DispatcherService.RunOnUIThreadAsync(() => UpdateAsyncInternal(item));

        private async Task UpdateAsyncInternal(ISyncItem<T> item)
        {
            if (UseSerializer)
            {
                string json = await SettingsService.GetValueAsync<string>(Key);
                item.Item = SerializationService.Deserialize<T>(json);
            }
            else
            {
                item.Item = await SettingsService.GetValueAsync<T>(Key);
            }
        }
    }

    /// <summary>
    /// Provides extension methods for registering <see cref="ISyncItem{T}"/>s to the DI container that are linked using <see cref="SettingsLink{T}"/> to a location in the settings store.
    /// </summary>
    public static class SettingsLinkExtensions
    {
        /// <summary>
        /// Registers <see cref="ISyncItem{T}"/> of the specified type <typeparamref name="T"/> to return a <see cref="SyncItem{T}"/> that is connected to the backing store of a specific location in settings using a <see cref="SettingsLink{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the context to provide. Generally, there should only be one declaration of this type of <see cref="ISyncItem{T}"/> per application.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="settingsKey">The <see cref="string"/> key or 'path' to the location in settings where the <typeparamref name="T"/> object should be stored.</param>
        public static void RegisterSettingsContext<T>(this ContainerBuilder builder, string settingsKey)
        {
            builder.RegisterType<SettingsLink<T>>()
                .WithProperty("Key", settingsKey)
                .Keyed<ILink<T>>($"Setting-{settingsKey}")
                .AsImplementedInterfaces();
            builder.RegisterType<SyncItem<T>>()
                .WithParameter(Autofac.Core.ResolvedParameter.ForKeyed<ILink<T>>($"Setting-{settingsKey}"))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
