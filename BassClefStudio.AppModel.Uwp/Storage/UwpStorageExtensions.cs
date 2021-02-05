using Windows.Storage;

namespace BassClefStudio.AppModel.Storage
{
    /// <summary>
    /// Provides extension methods for <see cref="IStorageItem"/> objects.
    /// </summary>
    public static class UwpStorageExtensions
    {
        /// <summary>
        /// Creates an MVVM <see cref="BassClefStudio.AppModel.Storage.IStorageItem"/> from a UWP <see cref="IStorageItem"/>.
        /// </summary>
        /// <param name="item">The <see cref="IStorageItem"/> to convert.</param>
        /// <returns>Either a <see cref="UwpFile"/> or <see cref="UwpFolder"/> wrapper around the item.</returns>
        public static BassClefStudio.AppModel.Storage.IStorageItem ToMvvm(this Windows.Storage.IStorageItem item)
        {
            if (item is Windows.Storage.IStorageFolder folder)
            {
                return new UwpFolder(folder);
            }
            else if (item is Windows.Storage.IStorageFile file)
            {
                return new UwpFile(file);
            }
            else
            {
                throw new BassClefStudio.AppModel.Storage.StorageAccessException("Accessed an item at the specified path, but it was not a file or directory.");
            }
        }

        public static FileAccessMode ToUwp(this FileOpenMode openMode)
        {
            switch(openMode)
            {
                case FileOpenMode.Read:
                    return FileAccessMode.Read;
                case FileOpenMode.ReadWrite:
                    return FileAccessMode.ReadWrite;
                default:
                    throw new StorageAccessException($"Failed to find the FileAccessMode to access the file content in mode {openMode}");
            }
        }

        public static CreationCollisionOption ToUwp(this CollisionOptions options)
        {
            switch (options)
            {
                case CollisionOptions.FailIfExists:
                    return CreationCollisionOption.FailIfExists;
                case CollisionOptions.OpenExisting:
                    return CreationCollisionOption.OpenIfExists;
                case CollisionOptions.Overwrite:
                    return CreationCollisionOption.ReplaceExisting;
                case CollisionOptions.RenameIfExists:
                    return CreationCollisionOption.GenerateUniqueName;
                default:
                    throw new StorageAccessException($"Failed to find the CreationCollisionOption to deal with file collisions for {options}");
            }
        }
    }
}
