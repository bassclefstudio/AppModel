﻿using BassClefStudio.AppModel.Commands;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// An <see cref="TriggerAction{T}"/> that can be declared in XAML to trigger a <see cref="CommandInfo"/> command using the default <see cref="Commands.CommandRouter"/> router.
    /// </summary>
    public class CallCommandAction : TriggerAction<DependencyObject>
    {
        /// <summary>
        /// The <see cref="CommandInfo"/> of the command to trigger, or 'null'.
        /// </summary>
        public CommandInfo? Command
        {
            get { return (CommandInfo?)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(CommandInfo?), typeof(CallCommandAction), new PropertyMetadata(null));

        /// <summary>
        /// The optional <see cref="object"/> parameter to send in the <see cref="CommandRequest"/> along with the <see cref="Command"/>.
        /// </summary>
        public object Parameter
        {
            get { return (object)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Parameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(object), typeof(CallCommandAction), new PropertyMetadata(null));

        /// <inheritdoc/>
        protected override void Invoke(object parameter)
        {
            if (Command.HasValue)
            {
                Commands.CommandRouter.Execute(this.Command.Value, this.Parameter);
            }
        }
    }
}