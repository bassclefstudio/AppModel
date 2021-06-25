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
        #region Commands

        /// <summary>
        /// A command for selecting <see cref="NavigationItem"/>s to navigate to.
        /// </summary>
        public static CommandInfo NavigateCommand { get; } = new CommandInfo()
        {
            Id = "Shell/Navigate",
            FriendlyName = "Navigate",
            Description = "Navigate to the specified page."
        };

        /// <summary>
        /// A command for triggering available back navigation.
        /// </summary>
        public static CommandInfo BackCommand { get; } = new CommandInfo()
        {
            Id = "Shell/Back",
            FriendlyName = "Go back",
            Description = "Return to the previously visited page."
        };

        /// <inheritdoc/>
        public ICommand[] Commands { get; }

        #endregion
        #region Properties

        /// <summary>
        /// A <see cref="SourceStream{T}"/> of requests to navigate to the settings page (see <see cref="SettingsItem"/>).
        /// </summary>
        public SourceStream<bool> SettingsStream { get; }

        /// <summary>
        /// The collection of all <see cref="NavigationItem"/>s available to this <see cref="ShellViewModel"/>.
        /// </summary>
        public ObservableCollection<NavigationItem> NavigationItems { get; }

        /// <summary>
        /// Retrieves an array of <see cref="NavigationItem"/>s that will be initially populated to the <see cref="NavigationItems"/> collection.
        /// </summary>
        protected abstract NavigationItem[] GetInitialItems();

        /// <summary>
        /// The <see cref="NavigationItem"/> for the settings page.
        /// </summary>
        public abstract NavigationItem SettingsItem { get; }

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

        #endregion
        #region Initialize

        /// <summary>
        /// The injected <see cref="INavigationService"/>.
        /// </summary>
        protected INavigationService NavigationService { get; }

        /// <summary>
        /// The injected <see cref="INavigationStack"/>.
        /// </summary>
        protected INavigationStack NavigationStack { get; }

        /// <summary>
        /// Creates a new <see cref="ShellViewModel"/>.
        /// </summary>
        public ShellViewModel(INavigationService navService, INavigationStack navStack)
        {
            NavigationService = navService;
            NavigationStack = navStack;

            NavigationItems = new ObservableCollection<NavigationItem>(GetInitialItems());
            
            var navigate = new StreamCommand(ShellViewModel.NavigateCommand);
            navigate.OfType<object, NavigationItem>().BindResult(Navigate);

            BackEnabled = NavigationStack.AsStream().Property(s => s.CanGoBack);
            
            var back = new StreamCommand(ShellViewModel.BackCommand, BackEnabled);
            back.OfType<object, bool>().BindResult(b => NavigationService.GoBack(NavigationStack));

            Commands = new ICommand[] { navigate, back };

            NavigationStack.RequestStream.BindResult(r =>
                SetSelected(NavigationItems.FirstOrDefault(i => i.Request == r)));

            SettingsStream = new SourceStream<bool>();
            SettingsStream.BindResult(b => Navigate(SettingsItem));
        }

        /// <inheritdoc/>
        public abstract Task InitializeAsync(object parameter = null);

        #endregion
        #region Actions

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

        #endregion
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
