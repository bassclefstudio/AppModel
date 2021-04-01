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
    public class ConsoleNavigationService : NavigationService<IConsoleView>, INavigationService
    {
        private string AppName { get; }
        private Version Version { get; }
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
        public override void InitializeNavigation()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{AppName} v{Version}");
            Console.ResetColor();
        }

        /// <inheritdoc/>
        protected override bool NavigateInternal(IConsoleView view)
        {
            SynchronousTask syncTask = new SynchronousTask(
                    () => view.ShowView());
            syncTask.RunTask();
            return true;
        }
    }
}
