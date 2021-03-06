﻿using BassClefStudio.AppModel.Commands;
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
        public static CommandInfo<NavigationItem> NavigateCommand { get; } = new CommandInfo<NavigationItem>()
        {
            Id = "Shell/Navigate",
            FriendlyName = "Navigate",
            Description = "Navigate to the specified page.",
            InputDescription = "The NavigationItem of the location to navigate to."
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
        public IList<ICommand> Commands { get; }

        #endregion
        #region Properties

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
        public NavigationItem SelectedItem { get => selected; set => Set(ref selected, value); }

        #endregion
        #region Initialize

        /// <summary>
        /// The injected <see cref="INavigationService"/>.
        /// </summary>
        protected INavigationService NavigationService { get; }

        /// <summary>
        /// Creates a new <see cref="ShellViewModel"/>.
        /// </summary>
        public ShellViewModel(INavigationService navStack)
        {
            NavigationService = navStack;

            NavigationItems = new ObservableCollection<NavigationItem>(GetInitialItems());
            
            var navigate = new StreamCommand<NavigationItem>(ShellViewModel.NavigateCommand);
            navigate.BindResult(Navigate);

            BackEnabled = NavigationService.History.RequestStream
                .Select(r => NavigationService.History.CanGoBack);
            
            var back = new StreamCommand(ShellViewModel.BackCommand, BackEnabled);
            back.BindResult(b => NavigationService.GoBack());

            Commands = new List<ICommand>() { navigate, back };
            
            NavigationService.History.RequestStream
                .Select(r => NavigationService.History.GetActiveViewModelType())
                .BindResult(t => SelectedItem = (NavigationItems.FirstOrDefault(i => i.Request.ViewModelType == t)));
        }

        /// <inheritdoc/>
        public abstract Task InitializeAsync(object parameter = null);

        #endregion
        #region Actions

        /// <summary>
        /// Navigates the <see cref="App"/> to the specified page.
        /// </summary>
        public void Navigate(NavigationItem item)
        {
            SelectedItem = item;
            NavigationService.Navigate(item.Request);
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
