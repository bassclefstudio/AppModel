using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    public class ConsoleStorageService : IStorageService
    {
        public IFolder AppDataFolder { get; }

        public ConsoleStorageService()
        {
            AppDataFolder = new ConsoleFolder(new DirectoryInfo(Directory.GetCurrentDirectory()));
        }

        /// <inheritdoc/>
        public async Task<IFile> RequestFileOpenAsync(StorageDialogSettings settings)
        {
            Console.Write($"{settings.OverrideSelectText ?? "Open"} ({string.Join(",", settings.ShownFileTypes.Select(f => $".{f}"))}): ");
            string path = Console.ReadLine();
            if (!settings.ShownFileTypes.Contains(Path.GetExtension(path)))
            {
                Debug.WriteLine("File found, but with the incorrect file extension.");
                return null;
            }
            return new ConsoleFile(new FileInfo(path));
        }

        /// <inheritdoc/>
        public async Task<IFile> RequestFileSaveAsync(StorageDialogSettings settings)
        {
            Console.Write($"{settings.OverrideSelectText ?? "Save"} ({string.Join(",", settings.ShownFileTypes.Select(f => $".{f}"))}): ");
            string path = Console.ReadLine();
            if(!settings.ShownFileTypes.Contains(Path.GetExtension(path)))
            {
                path = Path.ChangeExtension(path, settings.ShownFileTypes.FirstOrDefault());
            }
            var folder = new ConsoleFolder(new DirectoryInfo(Path.GetDirectoryName(path)));
            return await folder.CreateFileAsync(Path.GetFileName(path));
        }

        /// <inheritdoc/>
        public async Task<IFolder> RequestFolderAsync(StorageDialogSettings settings)
        {
            Console.Write($"{settings.OverrideSelectText ?? "Open Folder"}:");
            string path = Console.ReadLine();
            return new ConsoleFolder(new DirectoryInfo(Path.GetDirectoryName(path)));
        }
    }
}
