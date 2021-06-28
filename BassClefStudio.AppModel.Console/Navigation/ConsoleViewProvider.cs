using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// An <see cref="IViewProvider"/> built on the <see cref="ConsoleView{T}"/> abstract class.
    /// </summary>
    public class ConsoleViewProvider : ViewProvider<IConsoleView>, IViewProvider
    {
        private string AppName { get; }
        private Version Version { get; }
        /// <summary>
        /// Creates a new <see cref="ConsoleViewProvider"/>.
        /// </summary>
        /// <param name="packageInfo">The <see cref="IPackageInfo"/> this <see cref="ConsoleViewProvider"/> uses to get <see cref="AppName"/> and <see cref="Version"/> info.</param>
        public ConsoleViewProvider(IPackageInfo packageInfo)
        {
            AppName = packageInfo.ApplicationName;
            Version = packageInfo.Version;
        }

        /// <inheritdoc/>
        public override void StartUI()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{AppName} v{Version}");
            Console.ResetColor();
        }

        /// <inheritdoc/>
        protected override void SetViewInternal(NavigationRequest request, IConsoleView view)
        {
            if (request.Properties.LayerMode == LayerBehavior.Default)
            {
                SynchronousTask syncTask = new SynchronousTask(
                        () => view.ShowView());
                syncTask.RunTask();
            }
            else
            {
                throw new ArgumentException($"Console apps currently do not have support for navigation layers.", nameof(request));
            }
        }
    }
}
