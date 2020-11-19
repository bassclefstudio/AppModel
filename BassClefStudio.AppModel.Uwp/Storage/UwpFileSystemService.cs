using BassClefStudio.NET.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BassClefStudio.AppModel.Storage.Uwp
{
    public class UwpFileSystemService : IStorageService
    {
        public IFolder AppDataFolder { get; } = new UwpFolder(ApplicationData.Current.LocalFolder);

        public async Task<IFile> RequestFileOpenAsync(StorageDialogSettings settings)
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

        public async Task<IFile> RequestFileSaveAsync(StorageDialogSettings settings)
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

        public async Task<IFolder> RequestFolderAsync(StorageDialogSettings settings)
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
