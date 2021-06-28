using Windows.UI.Xaml.Controls;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents any <see cref="IView"/> that, in the UWP framework, hosts a <see cref="ContentControl"/> in which a new <see cref="INavigationLayer"/> of content will be provided (see <see cref="LayerBehavior.Shell"/>).
    /// </summary>
    public interface ILayerContainer : IView
    {
        /// <summary>
        /// The <see cref="ContentControl"/> child in which new content for this <see cref="INavigationLayer"/> will be shown.
        /// </summary>
        ContentControl Container { get; }
    }
}
