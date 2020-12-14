# AppModel
![.NET 5 Build and Test](https://github.com/bassclefstudio/AppModel/workflows/.NET%205%20Build%20and%20Test/badge.svg) ![MSBuild Build and Pack](https://github.com/bassclefstudio/AppModel/workflows/MSBuild%20Build%20and%20Pack/badge.svg)

A .NET Standard, platform-agnostic library for creating cross-platform view-models and applications for various .NET platforms. Go check out the sample project [here](https://github.com/bassclefstudio/RssReader) or read the tutorial, below, to get started with the BassClefStudio.AppModel libraries.

# Get Started
There are three packages that make up the BassClefStudio.AppModel project:
 - `BassClefStudio.AppModel`
 - `BassClefStudio.AppModel.Base`
 - `BassClefStudio.AppModel.Uwp`
 - `BassClefStudio.AppModel.Wpf`
 - `BassClefStudio.AppModel.Blazor`

In adddition, Xamarin packages are on the roadmap, as well as a possible `.Console`/`.Gtk` package.

The main package, `BassClefStudio.AppModel`, is a .NET Standard library that contains interfaces and basic code defining how an app should run. It handles navigation, file system access, dependency injection, and the MVVM platform (among other things) and is the basis of the AppModel project.

## Creating the base project
Create a .NET Standard project (2.0 or higher), either using the `dotnet` templates or Visual Studio. Once you create the project, you'll need to reference the `BassClefStudio.AppModel` Nuget package (available on this repo).

First, you'll need to create your App class. This is a class that defines the services and features that this specific app will require, and provides a starting point for developing an `AppModel` app.

**App.cs**
````C#
using Autofac;
using BassClefStudio.AppModel.Background;
using BassClefStudio.AppModel.Lifecycle;

public class MyApp : App
{
    public MyApp() : base("MyAppName")
    { }

    protected override void ConfigureServices(ContainerBuilder builder)
    {
        builder.RegisterViewModels(typeof(MyApp).Assembly);
        builder.RegisterAssemblyTypes(typeof(MyApp).Assembly)
            .AssignableTo<IBackgroundTask>()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterAssemblyTypes(typeof(MyApp).Assembly)
            .AssignableTo<IMyService>()
            .AsImplementedInterfaces();
    }
}
````
The base `App` class exposes the `ConfigureServices()` method, which allows you to register your app's view-models, background tasks, and any other services which you wish to use in your view-models. The app will create an `Autofac` DI container and use that to... resolve dependencies. If you're confused about how Autofac's syntax works, check out [their documentation](https://autofaccn.readthedocs.io/en/latest/). Their package is currently a dependency and requirement of the `AppModel` framework.

## `IViewModel` and `IActivationHandler`

Now you can create a view-model in the .NET Standard project. Here's an example of what that might look like, at the bare minimum.

**MainViewModel.cs**
````C#
using BassClefStudio.AppModel.Lifecycle;
public class MainViewModel : IViewModel
{
    public MainViewModel()
    { }

    public async Task InitializeAsync()
    { }
}
````

It declares that it implements `IViewModel` and contains an asynchronous `Task` `InitializeAsync()` which will be run each time this view-model (and associated view) is navigated to. However, this view-model doesn't do very much. First, let's tell the AppModel framework that this should be the default view-model to launch the app to.

````C#
...
public class MainViewModel : IViewModel, IActivationHandler
...
````

The `IActivationHandler` interface says that this `IViewModel` will support being the startup page, if you will, for certain activation circumstances. These are passed to the `IActivationHandler` in the form of `IActivatedEventArgs`, which (as of v1.2.0) come in the following default types (others may be added by you or by a specific platform):

|Name|Description|Properties|
|---|---|---|
|`LaunchActivatedEventArgs`|The app has been launched normally, with UI, by the user clicking on the app's icon on their home screen, start screen, or similar.|`string[] Args`|
|`StorageActivatedEventArgs`|The app has been launched to handle a specific file.|`IStorageItem AttachedItem`|
|`BackgroundActivatedEventArgs`|The app has been triggered by the system or platform to execute a specified background task.|`string TaskName`, `IDeferral Deferral`|

> **Note:** The `BackgroundActivatedEventArgs`, as a general rule, is handled by the base `App` class itself, and will not ask for any UI to be triggered. Creating an `IViewModel` that implements `IActivationHandler` will _not_ allow you to handle these activations.

Create override methods in your `IActivationHandler`, as so:

**MainViewModel.cs**
````C#
using BassClefStudio.AppModel.Lifecycle;
public class MainViewModel : IViewModel, IActivationHandler
{
    public bool CanHandle(IActivatedEventArgs args)
    {
        return args is LaunchActivatedEventArgs;
    }

    public void Activate(IActivatedEventArgs args)
    {

    }
}
````

We'll be handling `LaunchActivatedEventArgs` in this example, but by changing the code in `CanHandle(IActivatedEventArgs args)` you can return `true` for whatever activation you want.

If your view-model is the first registered, enabled (`Enable` property of your `IActivationHandler` set to `true`) view-model that can handle this activation, then the app will setup the navigation stack and navigate to `MainViewModel` and its associated view (more on views later). The `Activate` method will be called **before** navigation is completed, and the `Initialize` asynchronous task will (like it will in every `IViewModel`) be started after navigation is completed.

Great! we've successfully created a .NET Standard project with a view-model that will be navigated to when the app is launched. Now, we can work on creating our app logic in the view-model. The only problem is _how can we use platform-specific code (for something as simple as showing a file picker or saving some user settings) in our .NET Standard view-model?_

## Services to the Rescue
The answer is **services**! Services are interfaces that are implemented by the platform that you eventually run your app on. Take the `IStorageService` as an example. In `BassClefStudio.AppModel`, the interface is defined as a way to manage app data and file pickers, and various methods and properties are defined. Then, each platform creates its own implementation - `BassClefStudio.AppModel.Uwp` has a `UwpStorageService`, for example. Each platform head will tell your `MyApp` class to start and provide an `IAppPlatform` instance, which tells the DI container which concrete services that platform provides, and the base `App` code will register those types for your view-models to use and consume. Want to see some code?

**MainViewModel.cs**
````C#
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Storage;
public class MainViewModel : IViewModel, IActivationHandler
{
    ...
    private IStorageService StorageService;
    public MainViewModel(IStorageService storageService)
    {
        StorageService = storageService;
    }
}
````

It's as easy as that! Now your .NET Standard view-model can use the `IStorageService` to prompt the user for a file, or save configuration information to the local data folder. Each platform will simply give that view-model whatever `IStorageService` it provides through dependency injection. The following services are currently provided, though more are planned. All services have full XML documentation on all methods and public properties.

 - The `App` - this will return (in this case) the current `MyApp` instance, which has methods on it such as `Navigate<ViewModelType>()` which you can use for lifecycle and navigation.
 - Storage
    - `IStorageService` - file pickers and local data.
    - `IFile` and `IFolder` - types that an `IStorageService` can provide, which abstract over the file IO logic, etc.
 - Settings
    - `ISettingsService` - allows for apps to read/write user settings, which are either stored in a platform's settings store or as a JSON file in local data (see `BaseSettingsService`).
 - Navigation
    - `INavigationService` (internal use) - provides methods for `App` logic to navigate between `IView` instances.
 - Notifications
    - `INotificationService` - allows apps to send native platform notifications to the action center / notification list / other.
 - Threading
    - `IDispatcherService` - allows view-models and models to specify that code should be run on the same thread as the UI, which is required for some platforms (UWP and WPF, with data binding) when what is being changed in the view-model is being directly displayed/bound to.

In addition to services, .NET Standard projects have full access to the .NET Standard APIs, including those for networking (such as `HttpClient`), serialization (`Newtonsoft.Json`, etc.), and many other capabilities. These dependencies can either be used directly or added to the DI container in your app's `ConfigureServices()` method.

# Create a Platform Head
Now that you have models/view-models written in .NET Standard, it's time to connect your app up to a specific platform. This is designed to be as simple and decoupled as possible, so you can easily mix-and-match platforms as you develop; however, the instructions on setting up each platform differ, so the guidance below will be platform-specific.

## UWP (C#)
Create a new C# UWP project, with a minimum version of Windows 10 v1709 or higher (to support .NET Standard 2.0). Add a reference to _your_ .NET Standard model/view-model project, as well as the `BassClefStudio.AppModel.Uwp` Nuget package (should be same/similar version to the `BassClefStudio.AppModel` package). Then, replace the code in App.xaml.cs with the following:

**App.xaml.cs**
````C#
using BassClefStudio.AppModel.Lifecycle;
sealed partial class App : UwpApplication
{
    public App() : base(new MyApp(), typeof(App).Assembly)
    { }
}
````

> **Note:** In order for this to compile in a XAML project, you'll need to go into the XAML and replace the root `<App>` with `<lifecycle:UwpApplication>` and including the namespace `xmlns:lifecycle="using:BassClefStudio.AppModel.Lifecycle"`.

This `UwpApplication` class is provided by the `BassClefStudio.AppModel.Uwp` package and handles the UWP system events by calling the correct methods on your `MyApp : BassClefStudio.AppModel.Lifecycle.App` class. The `typeof(App).Assembly` argument provides an assembly where the relevant views for your UWP project will be found (you can specify as many assemblies as needed).

Now comes views (that's right, that's all you need for your app class!). Since `AppModel` libraries don't provide UI, you'll be creating your own UI in XAML/code-behind/etc. for UWP. In the code-behind for your view, include the following:

**MainPage.xaml.cs**
````C#
using BassClefStudio.AppModel.Navigation;
public sealed partial class MainPage : Page, IView<MainViewModel>
{
    public MainViewModel ViewModel { get; set; }

    public void Initialize()
    { }
}
````
> **Note:** If your view is a `ContentDialog`, navigation methods for UWP will automatically call the correct `ContentDialog.ShowAsync()` method instead of navigating the `Frame`/setting the content of the window, without you needing to do anything extra!

You've now told your app that you want this page `MainPage` to be connected with your `MainViewModel` - this means that this page will be navigated to when you call `MyApp.Navigate<MainViewModel>()` or another equivalent navigation statement. It will also be navigated to on application launch if you've registered the `MainViewModel` as an `IActivationHandler`.

> **Note:** Pages/'views' in `AppModel` apps can _also_ use the DI container - simply request a service in the constructor in exactly the same way. This is considered preferrable than dealing with the `Uwp*` services directly, since these may change over time (the services are all registered as interfaces anyway).

The `Initialize` method will be run after navigation is completed and the `ViewModel` property on the view is set, and can be used to call methods on the view-model if you really need to. Use this as your `OnNavigationCompleted` handler.

At this point, your UWP application should launch and activate to your specified `MainPage` - services should work when called and navigation should bring up the desired connected UWP views. Notice a problem with the library or this tutorial? Submit an issue [here](https://github.com/bassclefstudio/AppModel/issues/new).

## WPF (C#)
Create a new C# WPF project targeting .NET (Core) 5 or higher (to support the `.Wpf` library). Add a reference to _your_ .NET Standard model/view-model project, as well as the `BassClefStudio.AppModel.Wpf` Nuget package (should be same/similar version to the `BassClefStudio.AppModel` package, and will bring in `BassClefStudio.AppModel.Base` as a dependency). Then, replace the code in App.xaml.cs with the following:

**App.xaml.cs**
````C#
using BassClefStudio.AppModel.Lifecycle;
sealed partial class App : WpfApplication
{
    public App() : base(new MyApp(), typeof(App).Assembly)
    { }
}
````

> **Note:** In order for this to compile in a XAML project, you'll need to go into the XAML and replace the root `<App>` with `<lifecycle:WpfApplication>` and including the namespace `xmlns:lifecycle="clr-namespace:BassClefStudio.AppModel.Lifecycle;assembly=BassClefStudio.AppModel.Wpf"`.

This `WpfApplication` class is provided by the `BassClefStudio.AppModel.Wpf` package and handles the WPF system events by calling the correct methods on your `MyApp : BassClefStudio.AppModel.Lifecycle.App` class. The `typeof(App).Assembly` argument provides an assembly where the relevant views for your WPF project will be found (you can specify as many assemblies as needed).

Now comes views (that's right, that's all you need for your app class!). Since `AppModel` libraries don't provide UI, you'll be creating your own UI in XAML/code-behind/etc. for WPF. In the code-behind for your view, include the following:

**MainPage.xaml.cs**
````C#
using BassClefStudio.AppModel.Navigation;
public sealed partial class MainPage : Page, IView<MainViewModel>
{
    public MainViewModel ViewModel { get; set; }

    public void Initialize()
    {
        this.DataContext = ViewModel;
    }
}
````
> **Note:** If your view is a `Window`, navigation methods for WPF will automatically call the correct `Window.Show()` method instead of navigating the `Frame`/setting the content of the existing window, without you needing to do anything extra!

You've now told your app that you want this page `MainPage` to be connected with your `MainViewModel` - this means that this page will be navigated to when you call `MyApp.Navigate<MainViewModel>()` or another equivalent navigation statement. It will also be navigated to on application launch if you've registered the `MainViewModel` as an `IActivationHandler`.

> **Note:** Pages/'views' in `AppModel` apps can _also_ use the DI container - simply request a service in the constructor in exactly the same way. This is considered preferrable than dealing with the `Uwp*` services directly, since these may change over time (the services are all registered as interfaces anyway).

The `Initialize` method will be run after navigation is completed and the `ViewModel` property on the view is set, and can be used to call methods on the view-model if you really need to. Use this as your `OnNavigationCompleted` handler.

At this point, your WPF application should launch and activate to your specified `MainPage` - services should work when called and navigation should bring up the desired connected WPF views. Notice a problem with the library or this tutorial? Submit an issue [here](https://github.com/bassclefstudio/AppModel/issues/new).

## Blazor (C#)
_Coming soon!_
