using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// An <see cref="IValueConverter"/> for converting between <see cref="Visibility"/> and <see cref="bool"/> values.
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// A <see cref="bool"/> indicating whether the input <see cref="bool"/> values should be inverted before converting them into true/<see cref="Visibility.Visible"/> and false/<see cref="Visibility.Collapsed"/>.
        /// </summary>
        public bool IsInverted { get; set; }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is bool b)
            {
                return b == !IsInverted ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if(value is Visibility v)
            {
                return v == Visibility.Visible ? !IsInverted : IsInverted;
            }
            else
            {
                return false;
            }
        }
    }
}
