using BassClefStudio.AppModel.Threading;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Provides UWP methods for file system access.
    /// </summary>
    public class UwpStorageService : IStorageService
    {
        /// <inheritdoc/>
        public IStorageFolder AppDataFolder { get; } = new UwpFolder(ApplicationData.Current.LocalFolder);

        /// <inheritdoc/>
        public IStorageFolder TempFolder { get; } = new UwpFolder(ApplicationData.Current.TemporaryFolder);

        private IEnumerable<IDispatcher> Dispatchers { get; }
        public UwpStorageService(IEnumerable<IDispatcher> dispatchers)
        {
            Dispatchers = dispatchers;
        }

        /// <inheritdoc/>
        public async Task<IStorageFile> RequestFileOpenAsync(StorageDialogSettings settings)
        {
            FileOpenPicker dialog = new FileOpenPicker();
            if (settings.ShownFileTypes == null || !settings.ShownFileTypes.Any())
            {
                dialog.FileTypeFilter.Add("*");
            }
            else
            {
                dialog.FileTypeFilter.AddRange(settings.ShownFileTypes.Select(f => $".{f}"));
            }

            if (settings.OverrideSelectText != null)
            {
                dialog.CommitButtonText = settings.OverrideSelectText;
            }

            var file = await dialog.PickSingleFileAsync();
            return new UwpFile(file);
        }

        /// <inheritdoc/>
        public async Task<IStorageFile> RequestFileSaveAsync(StorageDialogSettings settings)
        {
            FileSavePicker dialog = new FileSavePicker();
            if (settings.ShownFileTypes == null || !settings.ShownFileTypes.Any())
            {
            }
            else
            {
                foreach (var type in settings.ShownFileTypes.Select(f => $".{f}"))
                {
                    dialog.FileTypeChoices.Add(type, new string[] { type });
                }
            }

            if (settings.OverrideSelectText != null)
            {
                dialog.CommitButtonText = settings.OverrideSelectText;
            }

            var file = await dialog.PickSaveFileAsync();
            return new UwpFile(file);
        }

        /// <inheritdoc/>
        public async Task<IStorageFolder> RequestFolderAsync(StorageDialogSettings settings)
        {
            FolderPicker dialog = new FolderPicker();
            if (settings.ShownFileTypes == null || !settings.ShownFileTypes.Any())
            {
                dialog.FileTypeFilter.Add("*");
            }
            else
            {
                dialog.FileTypeFilter.AddRange(settings.ShownFileTypes.Select(f => $".{f}"));
            }

            if (settings.OverrideSelectText != null)
            {
                dialog.CommitButtonText = settings.OverrideSelectText;
            }

            var folder = await dialog.PickSingleFolderAsync();
            return new UwpFolder(folder);
        }
    }
}
