using BassClefStudio.AppModel.Lifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Provides .NET console methods for file system access.
    /// </summary>
    public class ConsoleStorageService : IStorageService
    {
        /// <inheritdoc/>
        public IFolder AppDataFolder { get; }

        /// <summary>
        /// Creates a new <see cref="ConsoleStorageService"/> from the current <see cref="App"/>
        /// </summary>
        /// <param name="packageInfo">The app's <see cref="IPackageInfo"/> provides information used to determine the location of the local folder.</param>
        public ConsoleStorageService(IPackageInfo packageInfo)
        {
            AppDataFolder = new BaseFolder(
                new DirectoryInfo(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        packageInfo.ApplicationName)));
        }

        /// <inheritdoc/>
        public async Task<IFile> RequestFileOpenAsync(StorageDialogSettings settings)
        {
            IEnumerable<string> fileTypes = settings.ShownFileTypes?.Select(t => $".{t}");
            if (fileTypes == null || !fileTypes.Any())
            {
                Console.Write($"Select file (.*): ");
            }
            else
            {
                Console.Write($"Select file ({string.Join(",", fileTypes)}): ");
            }
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            else
            {
                var fileInfo = new FileInfo(name);
                if(fileInfo.Exists && (fileTypes == null || !fileTypes.Any() || fileTypes.Contains(fileInfo.Extension)))
                {
                    Console.Write($"{settings.OverrideSelectText ?? "Open"} (y/N)? ");
                    if(string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        return new BaseFile(fileInfo);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Console.Write($"File {name} does not exist or is of incorrect type. Try again (y/N)?");
                    if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        return await RequestFileOpenAsync(settings);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public async Task<IFile> RequestFileSaveAsync(StorageDialogSettings settings)
        {
            IEnumerable<string> fileTypes = settings.ShownFileTypes?.Select(t => $".{t}");
            if (fileTypes == null || !fileTypes.Any())
            {
                Console.Write($"Select file (.*): ");
            }
            else
            {
                Console.Write($"Select file ({string.Join(",", fileTypes)}): ");
            }
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            else
            {
                var fileInfo = new FileInfo(name);
                if (!(fileTypes == null || !fileTypes.Any() || fileTypes.Contains(fileInfo.Extension)))
                {
                    if (!Path.HasExtension(fileInfo.FullName))
                    {
                        fileInfo = new FileInfo($"{name}.{fileTypes.First()}");
                    }
                    else
                    {
                        Console.Write($"File {name} is of the wrong type. Try again (y/N)?");
                        if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                        {
                            return await RequestFileSaveAsync(settings);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                if (fileInfo.Exists)
                {
                    Console.Write($"File {name} already exists. Overwrite (y/N)?");
                    if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        return new BaseFile(fileInfo);
                    }
                    else
                    {
                        Console.Write($"Try again (y/N)?");
                        if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                        {
                            return await RequestFileSaveAsync(settings);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    return new BaseFile(fileInfo);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<IFolder> RequestFolderAsync(StorageDialogSettings settings)
        {
            Console.Write($"Select folder: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            else
            {
                var dirInfo = new DirectoryInfo(name);
                if (dirInfo.Exists)
                {
                    Console.Write($"{settings.OverrideSelectText ?? "Open"} (y/N)? ");
                    if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        return new BaseFolder(dirInfo);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Console.Write($"Directory {name} does not exist. Try again (y/N)?");
                    if (string.Equals(Console.ReadLine(), "y", StringComparison.OrdinalIgnoreCase))
                    {
                        return await RequestFolderAsync(settings);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}
