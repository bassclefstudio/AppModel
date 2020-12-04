using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public IFolder AppDataFolder { get; }

        /// <summary>
        /// Creates a new <see cref="WpfStorageService"/> from the current <see cref="Lifecycle.App"/>
        /// </summary>
        /// <param name="app">The <see cref="Lifecycle.App"/> and its name provides information used to determine the location of the local folder.</param>
        public WpfStorageService(Lifecycle.App app)
        {
            AppDataFolder = new BaseFolder(
                new System.IO.DirectoryInfo(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        app.ApplicationName)));
        }

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
                throw new StorageAccessException("File dialog closed or failed to find file.");
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
                throw new StorageAccessException("File dialog closed or failed to find file.");
            }
        }

        /// <inheritdoc/>
        public async Task<IFolder> RequestFolderAsync(StorageDialogSettings settings)
        {
            throw new PlatformNotSupportedException("WPF does not currently provide an API for picking folders.");
        }
    }
}
