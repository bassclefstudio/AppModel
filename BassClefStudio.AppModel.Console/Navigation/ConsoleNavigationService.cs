using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    public class ConsoleNavigationService : INavigationService
    {
        private ConsoleView currentView;
        public ConsoleView CurrentView 
        {
            get => currentView;
            private set
            {
                if(currentView != null)
                {
                    currentView.Updated -= CurrentViewUpdated;
                }
                currentView = value;
                if (currentView != null)
                {
                    currentView.Updated += CurrentViewUpdated;
                }
                CurrentViewUpdated();
            } 
        }

        private void CurrentViewUpdated(object sender, EventArgs e) => CurrentViewUpdated();
        private void CurrentViewUpdated()
        {
            Console.Clear();
            if (CurrentView != null)
            {
                SynchronousTask drawTask = new SynchronousTask(CurrentView.DrawContentAsync);
                drawTask.RunTask();
            }
        }

        public event EventHandler Navigated;

        public void InitializeNavigation()
        {
            CurrentView = null;
        }

        public void Navigate(IView view, object parameter = null)
        {
            if(view is ConsoleView consoleView)
            {
                CurrentView = consoleView;
                Navigated?.Invoke(this, new EventArgs());
            }
            else
            {
                throw new ArgumentException($"Navigation failed: Console app navigation must navigate to IViews of type ConsoleView. View type: {view?.GetType().Name}");
            }
        }
    }
}
