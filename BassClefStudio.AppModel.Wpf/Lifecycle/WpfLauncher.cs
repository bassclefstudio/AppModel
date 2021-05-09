using BassClefStudio.AppModel.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// A WPF wrapper around Windows' support for <see cref="Process"/> calls to launch files, folders, and links.
    /// </summary>
    public class WpfLauncher : ILauncher
    {
        private IStorageService StorageService { get; }
        /// <summary>
        /// Creates a new <see cref="WpfLauncher"/> from the required services.
        /// </summary>
        public WpfLauncher(IStorageService storageService)
        {
            StorageService = storageService;
        }

        /// <inheritdoc/>
        public async Task<bool> OpenFileAsync(IStorageFile file)
        {
            if (file.HasPath)
            {
                Process.Start(file.GetPath());
                return true;
            }
            else
            {
                var newFile = await file.CopyToAsync(StorageService.TempFolder);
                if (newFile.HasPath)
                {
                    Process.Start(newFile.GetPath());
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ShowFileAsync(IStorageFile file)
        {
            if (file.HasPath)
            {
                string folderPath = Path.GetDirectoryName(file.GetPath());
                Process.Start("explorer.exe", folderPath);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ShowFolderAsync(IStorageFolder folder)
        {
            if (folder.HasPath)
            {
                Process.Start("explorer.exe", folder.GetPath());
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> OpenLinkAsync(Uri link)
        {
            Process.Start(link.ToString());
        }
    }
}
