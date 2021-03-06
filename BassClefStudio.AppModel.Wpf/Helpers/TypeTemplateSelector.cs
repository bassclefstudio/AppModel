﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// Represents a <see cref="DataTemplateSelector"/> that selects between one or more <see cref="DataTemplate"/>s based on the type of the items.
    /// </summary>
    [ContentProperty(nameof(Templates))]
    public class TypeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// A list of <see cref="TypeTemplateEntry"/> objects describing all available templates.
        /// </summary>
        public List<TypeTemplateEntry> Templates { get; set; }

        /// <summary>
        /// Creates a new <see cref="TypeTemplateSelector"/>.
        /// </summary>
        public TypeTemplateSelector()
        {
            Templates = new List<TypeTemplateEntry>();
        }

        /// <summary>
        /// Creates a new <see cref="TypeTemplateSelector"/>.
        /// </summary>
        /// <param name="templates">A collection of <see cref="TypeTemplateEntry"/> objects describing all available templates.</param>
        public TypeTemplateSelector(IEnumerable<TypeTemplateEntry> templates)
        {
            Templates = new List<TypeTemplateEntry>(templates);
        }

        /// <inheritdoc/>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return Templates.FirstOrDefault(t => item.GetType() == t.DataType)?.Template;
        }
    }

    /// <summary>
    /// Represents a single entry in the <see cref="TypeTemplateSelector"/>'s list of available templates.
    /// </summary>
    [ContentProperty(nameof(Template))]
    public class TypeTemplateEntry
    {
        /// <summary>
        /// The <see cref="Type"/> of data that should be presented with this <see cref="DataTemplate"/>.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// The <see cref="DataTemplate"/> template used to display data of type <see cref="DataType"/>.
        /// </summary>
        public DataTemplate Template { get; set; }
    }
}
