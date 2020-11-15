using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Mvvm.Services.Files
{
    /// <summary>
    /// Represents a service that can query and abstract over the platform's filesystem.
    /// </summary>
    public interface IFileSystemService
    {
        /// <summary>
        /// A reference to the <see cref="IFolder"/> where an app can store app data in the form of files.
        /// </summary>
        IFolder AppDataFolder { get; }

        /// <summary>
        /// Prompts the user to select a file from their local filesystem, and returns an <see cref="IFile"/> reference to that file.
        /// </summary>
        /// <param name="settings">A <see cref="StorageDialogSettings"/> describing the appearance and filters of the storage dialog.</param>
        /// <param name="openMode">A <see cref="FileOpenMode"/> value describing how the file should be opened. If <see cref="FileOpenMode.Read"/>, the method prompts with an 'open' dialog; <see cref="FileOpenMode.ReadWrite"/>, a 'save' dialog.</param>
        /// <exception cref="FileAccessException">Accessing the selected file failed, or the prompt threw an exception while browsing the filesystem.</exception>
        /// <returns>An <see cref="IFile"/> object if the operation succeeded, and null if the operation is canceled gracefully by the user. If any other error occurs opening the file, a <see cref="FileAccessException"/> is thrown.</returns>
        Task<IFile> RequestFileAsync(StorageDialogSettings settings, FileOpenMode openMode = FileOpenMode.Read);

        /// <summary>
        /// Prompts the user to select a folder/directory from their local filesystem, and returns an <see cref="IFolder"/> reference to that folder.
        /// </summary>
        /// <exception cref="FileAccessException">Accessing the selected folder failed, or the prompt threw an exception while browsing the filesystem.</exception>
        /// <returns>An <see cref="IFile"/> object if the operation succeeded, and null if the operation is canceled gracefully by the user. If any other error occurs opening the file, a <see cref="FileAccessException"/> is thrown.</returns>
        Task<IFolder> RequestFolderAsync();
    }
    
    //// NOTE: In C# 9, this could be a record type.
    /// <summary>
    /// Represents a set of settings describing how a file dialog should behave.
    /// </summary>
    public class StorageDialogSettings
    {
        /// <summary>
        /// Overrides the default text displayed to the user when they choose the file or folder. If null, the default is used.
        /// </summary>
        public string OverrideSelectText { get; }

        /// <summary>
        /// An array of <see cref="string"/> file types, not including the preceding dot, that the storage dialog should display (e.g. ["cs", "csproj"]). If empty or null, all files are shown. 
        /// </summary>
        public string[] ShownFileTypes { get; }

        /// <summary>
        /// Creates a new <see cref="StorageDialogSettings"/>.
        /// </summary>
        /// <param name="overrideSelectText">Overrides the default text displayed to the user when they choose the file or folder.</param>
        /// <param name="shownFileTypes">An array of <see cref="string"/> file types, not including the preceding dot, that the storage dialog should display (e.g. ["cs", "csproj"]). By default, all files are shown.</param>
        public StorageDialogSettings(string overrideSelectText = null, string[] shownFileTypes = null)
        {
            OverrideSelectText = overrideSelectText;
            ShownFileTypes = shownFileTypes;
        }
    }
}
