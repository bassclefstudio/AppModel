using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// An <see cref="IValueConverter"/> that allows for the composition of multiple child <see cref="IValueConverter"/>s into one single <see cref="IValueConverter"/> that can be used in binding.
    /// </summary>
    [ContentProperty(Name = nameof(Converters))]
    public class CompositeConverter : IValueConverter
    {
        /// <summary>
        /// A collection, in order, of the <see cref="IValueConverter"/> components of this <see cref="CompositeConverter"/>.
        /// </summary>
        public List<IValueConverter> Converters { get; set; }

        /// <summary>
        /// Creates a new <see cref="CompositeConverter"/>.
        /// </summary>
        public CompositeConverter()
        {
            Converters = new List<IValueConverter>();
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var currrent = value;
            foreach (var con in Converters)
            {
                currrent = con.Convert(currrent, targetType, parameter, language);
            }
            return currrent;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var currrent = value;
            foreach (var con in Converters.Reverse<IValueConverter>())
            {
                currrent = con.ConvertBack(currrent, targetType, parameter, language);
            }
            return currrent;
        }
    }
}
