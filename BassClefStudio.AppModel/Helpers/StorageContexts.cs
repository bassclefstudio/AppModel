using Autofac;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Serialization;
using BassClefStudio.NET.Sync;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// An <see cref="ILink{T}"/> that provides a means for getting and storing data in a file using the <see cref="IStorageService"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects that this <see cref="ILink{T}"/> handles.</typeparam>
    public class StorageLink<T> : ILink<T>
    {
        /// <summary>
        /// The <see cref="string"/> path to the file in app storage where the <typeparamref name="T"/> item should be stored.
        /// </summary>
        public string Path { get; set; }

        internal IStorageService StorageService { get; }
        internal IDispatcherService DispatcherService { get; }
        internal ISerializationService SerializationService { get; }
        /// <summary>
        /// Creates a new <see cref="StorageLink{T}"/> from the required services.
        /// </summary>
        public StorageLink(IStorageService storageService, IDispatcherService dispatcherService, ISerializationService serializationService)
        {
            StorageService = storageService;
            DispatcherService = dispatcherService;
            SerializationService = serializationService;
        }

        /// <inheritdoc/>
        public async Task<bool> PushAsync(ISyncItem<T> item)
        {
            try
            {
                string json = SerializationService.Serialize(item.Item);
                var file = await StorageService.AppDataFolder.CreateFileAsync(Path);
                await file.WriteTextAsync(json);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Storage context failed: {ex}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(ISyncItem<T> item)
        {
            try
            {
                var file = await StorageService.AppDataFolder.CreateFileAsync(Path);
                string json = await file.ReadTextAsync();
                if (string.IsNullOrWhiteSpace(json))
                {
                    return false;
                }
                else
                {
                    await DispatcherService.RunOnUIThreadAsync(() =>
                    {
                        item.Item = SerializationService.Deserialize<T>(json);
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Storage context failed: {ex}");
                return false;
            }
        }
    }

    /// <summary>
    /// Provides extension methods for registering <see cref="ISyncItem{T}"/>s to the DI container that are linked using <see cref="StorageLink{T}"/> to a specific location in the app data folder.
    /// </summary>
    public static class StorageLinkExtensions
    {
        /// <summary>
        /// Registers <see cref="ISyncItem{T}"/> of the specified type <typeparamref name="T"/> to return a <see cref="SyncItem{T}"/> that is connected to the backing store of a specific location in the app data folder using a <see cref="StorageLink{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the context to provide. Generally, there should only be one declaration of this type of <see cref="ISyncItem{T}"/> per application.</typeparam>
        /// <param name="builder">The <see cref="ContainerBuilder"/> to add services to.</param>
        /// <param name="filePath">The <see cref="string"/> 'path' to the location in in app data folder where the <typeparamref name="T"/> object should be stored.</param>
        /// <param name="friendlyName">A <see cref="string"/> 'friendly name' to use to identify the <see cref="ILink{T}"/> to this specific file in the DI registration.</param>
        public static void RegisterStorageContext<T>(this ContainerBuilder builder, string filePath, string friendlyName = null)
        {
            string diName = friendlyName ?? filePath.Replace(Path.DirectorySeparatorChar, '-');
            builder.RegisterType<StorageLink<T>>()
                .WithProperty("Path", filePath)
                .Keyed<ILink<T>>($"File-{diName}")
                .AsImplementedInterfaces();
            builder.RegisterType<SyncItem<T>>()
                .WithParameter(Autofac.Core.ResolvedParameter.ForKeyed<ILink<T>>($"File-{diName}"))
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
