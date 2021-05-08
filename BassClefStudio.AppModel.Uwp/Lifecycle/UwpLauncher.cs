using BassClefStudio.AppModel.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// A UWP wrapper around <see cref="Windows.System.Launcher"/> which implements the <see cref="ILauncher"/> methods and also provides for the opening of non-UWP resources.
    /// </summary>
    public class UwpLauncher : ILauncher
    {
        private IStorageService StorageService { get; }
        /// <summary>
        /// Creates a new <see cref="UwpLauncher"/> from the required services.
        /// </summary>
        public UwpLauncher(IStorageService storageService)
        {
            StorageService = storageService;
        }

        /// <inheritdoc/>
        public async Task<bool> OpenFileAsync(IStorageFile file)
        {
            if (file is UwpFile uwpFile)
            {
                return await Launcher.LaunchFileAsync(uwpFile.File);
            }
            else
            {
                var newFile = await file.CopyToAsync(StorageService.TempFolder);
                if (newFile is UwpFile uwpNewFile)
                {
                    return await Launcher.LaunchFileAsync(uwpNewFile.File);
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
                return await Launcher.LaunchUriAsync(new Uri(file.GetPath()));
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ShowFolderAsync(IStorageFolder folder)
        {
            if (folder is UwpFolder uwpFolder)
            {
                return await Launcher.LaunchFolderAsync(uwpFolder.Folder);
            }
            else if(folder.HasPath)
            {
                return await Launcher.LaunchUriAsync(new Uri(folder.GetPath()));
            }
            else
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> OpenLinkAsync(Uri link)
        {
            return await Launcher.LaunchUriAsync(link);
        }
    }
}
