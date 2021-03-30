using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// An <see cref="INavigationService"/> built on the <see cref="ConsoleView{T}"/> abstract class.
    /// </summary>
    public class ConsoleNavigationService : INavigationService
    {
        internal string AppName { get; }
        internal Version Version { get; }

        /// <summary>
        /// Creates a new <see cref="ConsoleNavigationService"/>.
        /// </summary>
        /// <param name="packageInfo">The <see cref="IPackageInfo"/> this <see cref="ConsoleNavigationService"/> uses to get <see cref="AppName"/> and <see cref="Version"/> info.</param>
        public ConsoleNavigationService(IPackageInfo packageInfo)
        {
            AppName = packageInfo.ApplicationName;
            Version = packageInfo.Version;
        }

        /// <inheritdoc/>
        public void InitializeNavigation()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{AppName} v{Version}");
            Console.ResetColor();
        }

        /// <inheritdoc/>
        public void Navigate(IView view)
        {
            if (view is IConsoleView consoleView)
            {
                SynchronousTask syncTask = new SynchronousTask(
                    () => consoleView.ShowView());
                syncTask.RunTask();
            }
            else
            {
                throw new ArgumentException($"Console navigation usually expects that the resolved IViews be IConsoleViews. View type: {view?.GetType().Name}");
            }
        }
    }
}
