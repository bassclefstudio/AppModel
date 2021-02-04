using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Lifecycle
{
    /// <summary>
    /// Represents information about the <see cref="App"/>, including its name and version.
    /// </summary>
    public interface IPackageInfo
    {
        /// <summary>
        /// The name of the <see cref="App"/>.
        /// </summary>
        string ApplicationName { get; }

        /// <summary>
        /// The <see cref="System.Version"/> current version of this <see cref="App"/>.
        /// </summary>
        Version Version { get; }
    }

    /// <summary>
    /// Represents a basic struct implementation of the <see cref="IPackageInfo"/> interface.
    /// </summary>
    public struct PackageInfo : IPackageInfo
    {
        /// <summary>
        /// The name of the <see cref="App"/>.
        /// </summary>
        public string ApplicationName { get; internal set; }

        /// <summary>
        /// The <see cref="System.Version"/> current version of this <see cref="App"/>.
        /// </summary>
        public Version Version { get; internal set; }
    }
}
