using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Provides WPF methods for file system access.
    /// </summary>
    public class WpfStorageService : IStorageService
    {
        /// <inheritdoc/>
        public IFolder AppDataFolder { get; } = new BaseFolder(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)));

        /// <inheritdoc/>
        public async Task<IFile> RequestFileOpenAsync(StorageDialogSettings settings)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (settings.ShownFileTypes != null)
            {
                fileDialog.DefaultExt = $".{settings.ShownFileTypes.First()}";
                fileDialog.Filter = string.Join(",", settings.ShownFileTypes.Select(e => $".{e}"));
            }
            if (settings.OverrideSelectText != null)
            {
                fileDialog.Title = settings.OverrideSelectText;
            }

            if(fileDialog.ShowDialog() ?? false)
            {
                return new BaseFile(new System.IO.FileInfo(fileDialog.FileName));
            }
            else
            {
                Debug.WriteLine("File dialog closed or failed to find file.");
            }
        }

        /// <inheritdoc/>
        public async Task<IFile> RequestFileSaveAsync(StorageDialogSettings settings)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            if (settings.ShownFileTypes != null)
            {
                fileDialog.DefaultExt = $".{settings.ShownFileTypes.First()}";
                fileDialog.Filter = string.Join(",", settings.ShownFileTypes.Select(e => $".{e}"));
            }
            if (settings.OverrideSelectText != null)
            {
                fileDialog.Title = settings.OverrideSelectText;
            }

            if (fileDialog.ShowDialog() ?? false)
            {
                return new BaseFile(new System.IO.FileInfo(fileDialog.FileName));
            }
            else
            {
                Debug.WriteLine("File dialog closed or failed to find file.");
            }
        }

        /// <inheritdoc/>
        public async Task<IFolder> RequestFolderAsync(StorageDialogSettings settings)
        {
            throw new PlatformNotSupportedException("WPF does not currently provide an API for picking folders.");
        }
    }
}
