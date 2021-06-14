using BassClefStudio.AppModel.Commands;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.NET.Core;
using BassClefStudio.NET.Core.Streams;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Helpers
{
    /// <summary>
    /// An <see cref="IViewModel"/>/<see cref="IShellHandler"/> view-model that deals with managing navigation between pages.
    /// </summary>
    public abstract class ShellViewModel : Observable, IViewModel, IShellHandler
    {
        /// <summary>
        /// The collection of all <see cref="NavigationItem"/>s available to this <see cref="ShellViewModel"/>.
        /// </summary>
        public ObservableCollection<NavigationItem> NavigationItems { get; }

        /// <summary>
        /// Retrieves an array of <see cref="NavigationItem"/>s that will be initially populated to the <see cref="NavigationItems"/> collection.
        /// </summary>
        protected abstract NavigationItem[] GetInitialItems();

        /// <summary>
        /// An <see cref="ICommand{T}"/> for selecting <see cref="NavigationItem"/>s to navigate to.
        /// </summary>
        public ICommand<NavigationItem> NavigateCommand { get; }

        /// <summary>
        /// An <see cref="ICommand{T}"/> for triggering back navigation (if available).
        /// </summary>
        public ICommand<bool> BackCommand { get; }

        /// <summary>
        /// An <see cref="IStream{T}"/> that specifies whether back navigation is currently enabled/available, as a <see cref="bool"/>.
        /// </summary>
        public IStream<bool> BackEnabled { get; } 

        private NavigationItem selected;
        /// <summary>
        /// The currently selected <see cref="NavigationItem"/>.
        /// </summary>
        public NavigationItem SelectedItem 
        {
            get => selected;
            set
            {
                if (selected != value)
                {
                    Set(ref selected, value);
                    if (selected != null)
                    {
                        Navigate();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the value of <see cref="SelectedItem"/> without triggering navigation.
        /// </summary>
        private void SetSelected(NavigationItem item)
        {
            if (selected != item)
            {
                Set(ref selected, item, nameof(SelectedItem));
            }
        }

        /// <summary>
        /// The injected <see cref="INavigationService"/>.
        /// </summary>
        private INavigationService NavigationService { get; }

        /// <summary>
        /// The injected <see cref="INavigationStack"/>.
        /// </summary>
        private INavigationStack NavigationStack { get; }

        /// <summary>
        /// Creates a new <see cref="ShellViewModel"/>.
        /// </summary>
        public ShellViewModel(INavigationService navService, INavigationStack navStack)
        {
            NavigationService = navService;
            NavigationStack = navStack;

            NavigationItems = new ObservableCollection<NavigationItem>(GetInitialItems());
            NavigateCommand = new StreamCommand<NavigationItem>(
                new CommandInfo()
                {
                    Id = "Shell/Navigate",
                    FriendlyName = "Navigate",
                    Description = "Navigate to the specified page."
                });
            NavigateCommand.BindResult(Navigate);

            BackEnabled = NavigationStack.AsStream().Property(s => s.CanGoBack);
            BackCommand = new StreamCommand<bool>(
                new CommandInfo()
                {
                    Id = "Shell/Back",
                    FriendlyName = "Go back",
                    Description = "Return to the previously visited page."
                },
                BackEnabled);
            BackCommand.BindResult(b => NavigationService.GoBack(NavigationStack));

            NavigationStack.RequestStream.BindResult(r =>
                SetSelected(NavigationItems.FirstOrDefault(i => i.Request == r)));
        }

        /// <inheritdoc/>
        public abstract Task InitializeAsync(object parameter = null);

        /// <summary>
        /// Navigates the <see cref="App"/> to the specified page (sets the <see cref="SelectedItem"/> property).
        /// </summary>
        public void Navigate(NavigationItem item)
        {
            SelectedItem = item;
        }

        /// <summary>
        /// Navigates the <see cref="App"/> to the selected page.
        /// </summary>
        public void Navigate()
        {
            NavigationService.Navigate(SelectedItem.Request);
        }
    }

    /// <summary>
    /// Represents an item describing a page that can be navigated to.
    /// </summary>
    public class NavigationItem
    {
        /// <summary>
        /// The display name of the <see cref="NavigationItem"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A <see cref="char"/> that represents the icon of this <see cref="NavigationItem"/>.
        /// </summary>
        public char Icon { get; }

        /// <summary>
        /// The <see cref="NavigationRequest"/> request associated with thi <see cref="NavigationItem"/>.
        /// </summary>
        public NavigationRequest Request { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationItem"/>.
        /// </summary>
        /// <param name="name">The display name of the <see cref="NavigationItem"/>.</param>
        /// <param name="request">The <see cref="NavigationRequest"/> request associated with thi <see cref="NavigationItem"/>.</param>
        /// <param name="icon">A <see cref="char"/> that represents the icon of this <see cref="NavigationItem"/>.</param>
        public NavigationItem(string name, NavigationRequest request, char icon)
        {
            Name = name;
            Request = request;
            Icon = icon;
        }
    }
}
