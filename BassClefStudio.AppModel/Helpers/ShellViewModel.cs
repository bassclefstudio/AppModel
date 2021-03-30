﻿using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Streams;
using BassClefStudio.NET.Core;
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
        /// A <see cref="SourceStream{T}"/> that emits selected <see cref="NavigationItem"/> items that have been selected in the UI.
        /// </summary>
        public SourceStream<NavigationItem> TriggerStream { get; }

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
            Set(ref selected, item, nameof(SelectedItem));
        }

        /// <summary>
        /// The injected <see cref="App"/>.
        /// </summary>
        public App MyApp { get; set; }

        /// <summary>
        /// The injected <see cref="INavigationService"/>, which can be queried to setup the shell view settings for a given platform.
        /// </summary>
        public INavigationService NavigationService { get; set; }

        /// <summary>
        /// Creates a new <see cref="ShellViewModel"/>.
        /// </summary>
        public ShellViewModel(App myApp)
        {
            NavigationItems = new ObservableCollection<NavigationItem>(GetInitialItems());
            MyApp = myApp;
            MyApp.Navigated += AppNavigated;
            TriggerStream = new SourceStream<NavigationItem>();
            TriggerStream.BindResult(Navigate);
            //// Starting an empty SourceStream doesn't actually *do* anything, but still...
            _ = TriggerStream.StartAsync();
        }

        /// <inheritdoc/>
        public abstract Task InitializeAsync(object parameter = null);

        private void AppNavigated(object sender, NavigatedEventArgs e)
        {
            var viewModelType = e.NavigatedViewModel.GetType();
            SetSelected(NavigationItems.FirstOrDefault(i => i.ViewModelType == viewModelType));
        }

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
            MyApp.NavigateReflection(SelectedItem.ViewModelType);
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
        /// The type of the view-model (<see cref="IViewModel"/>) that this <see cref="NavigationItem"/> refers to.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// A <see cref="char"/> that represents the icon of this <see cref="NavigationItem"/>.
        /// </summary>
        public char Icon { get; }

        /// <summary>
        /// Creates a new <see cref="NavigationItem"/>.
        /// </summary>
        /// <param name="name">The display name of the <see cref="NavigationItem"/>.</param>
        /// <param name="viewModelType">The type of the view-model (<see cref="IViewModel"/>) that this <see cref="NavigationItem"/> refers to.</param>
        /// <param name="icon">A <see cref="char"/> that represents the icon of this <see cref="NavigationItem"/>.</param>
        public NavigationItem(string name, Type viewModelType, char icon)
        {
            Name = name;
            ViewModelType = viewModelType;
            Icon = icon;
        }
    }
}
