﻿using Autofac;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Serialization;
using BassClefStudio.NET.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal ISettingsService SettingsService { get; }
        internal IEnumerable<IDispatcher> Dispatchers { get; }
        internal ISerializationService SerializationService { get; }
        /// <summary>
        /// Creates a new <see cref="SettingsLink{T}"/> from the required services and optional <see cref="ISerializationService"/>.
        /// </summary>
        public SettingsLink(ISettingsService settingsService, IEnumerable<IDispatcher> dispatchers, ISerializationService serializationService)
        {
            SettingsService = settingsService;
            Dispatchers = dispatchers;
            SerializationService = serializationService;
        }

        /// <inheritdoc/>
        public async Task<bool> PushAsync(ISyncItem<T> item)
        {
            try
            {
                string json = SerializationService.Serialize(item.Item);
                await SettingsService.SetValueAsync(Key, json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Settings context failed: {ex}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(ISyncItem<T> item)
            => await Dispatchers.RunOnUIThreadAsync(() => UpdateAsyncInternal(item));

        private async Task<bool> UpdateAsyncInternal(ISyncItem<T> item)
        {
            try
            {
                string json = await SettingsService.GetValueAsync<string>(Key);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return false;
                }
                else
                {
                    item.Item = SerializationService.Deserialize<T>(json);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Settings context failed: {ex}");
                return false;
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
