using Autofac;
using BassClefStudio.AppModel.Background;
using BassClefStudio.AppModel.Lifecycle;
using BassClefStudio.AppModel.Navigation;
using BassClefStudio.AppModel.Notifications;
using BassClefStudio.AppModel.Settings;
using BassClefStudio.AppModel.Storage;
using BassClefStudio.AppModel.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Tests
{
    [TestClass]
    public class ContainerTests
    {
        public App DefaultApp { get; set; }
        public IAppPlatform AppPlatform { get; set; }

        [TestInitialize]
        public void CreateApps()
        {
            DefaultApp = new TestingApp();
            AppPlatform = new TestingAppPlatform(builder =>
            {
                builder.Register<IViewProvider>(c => Mock.Of<IViewProvider>())
                    .SingleInstance()
                    .AsImplementedInterfaces();
                builder.Register<IBackgroundService>(c => Mock.Of<IBackgroundService>())
                    .SingleInstance()
                    .AsImplementedInterfaces();
                builder.Register<IStorageService>(c => Mock.Of<IStorageService>())
                    .AsImplementedInterfaces();
                builder.Register<ISettingsService>(c => Mock.Of<ISettingsService>())
                    .AsImplementedInterfaces();
                builder.Register<IDispatcher>(c => Mock.Of<IDispatcher>())
                    .AsImplementedInterfaces();
                builder.Register<INotificationService>(c => Mock.Of<INotificationService>())
                    .AsImplementedInterfaces();
                builder.Register<ILauncher>(c => Mock.Of<ILauncher>())
                    .AsImplementedInterfaces();
            });
        }

        [TestMethod]
        public void CheckDefaultServices()
        {
            DefaultApp.Initialize(AppPlatform);
            CheckSingleResolved<INavigationService>();
            CheckSingleResolved<INavigationStack>();
            var package = CheckSingleResolved<IPackageInfo>("TestingApp package info");
            Assert.AreEqual(DefaultApp.PackageInfo, package, "Returned package info is not equivalent to the app's package info.");
            CheckResolved<ILifetimeScope>("DI container");
            CheckResolved<IBackHandler>("System back handler");
        }

        [TestMethod]
        public void CheckPlatformServices()
        {
            DefaultApp.Initialize(AppPlatform);
            CheckSingleResolved<IViewProvider>("UI service");
            CheckSingleResolved<IBackgroundService>("Background tasks");
            CheckResolved<IStorageService>();
            CheckResolved<ISettingsService>();
            CheckResolved<INotificationService>();
            CheckResolved<ILauncher>();
            CheckAllResolved<IDispatcher>("Dispatchers");
        }

        private T CheckResolved<T>(string key = null)
        {
            T service = DefaultApp.Services.Resolve<T>();
            Assert.IsNotNull(service, "Service resolved, but is null.");
            Console.WriteLine($"{typeof(T).Name}{(key == null ? string.Empty : $" (\"{key}\")")} resolved to {service.GetType().Name}");
            return service;
        }

        private T CheckSingleResolved<T>(string key = null)
        {
            T service = DefaultApp.Services.Resolve<T>();
            T s2 = DefaultApp.Services.Resolve<T>();
            Assert.IsNotNull(service, "Service resolved, but is null.");
            Assert.IsTrue(object.ReferenceEquals(service, s2), "The two resolved services are different instances, when they should be the same.");
            Console.WriteLine($"{typeof(T).Name}{(key == null ? string.Empty : $" (\"{key}\")")} resolved to singleton {service.GetType().Name}");
            return service;
        }

        private IEnumerable<T> CheckAllResolved<T>(string key = null)
        {
            IEnumerable<T> services = DefaultApp.Services.Resolve<IEnumerable<T>>();
            Assert.IsNotNull(services, "Services resolved, but is null.");
            Assert.AreNotEqual(0, services.Count(), "Service collection resolved, but none were found.");
            Console.WriteLine($"{typeof(T).Name}{(key == null ? string.Empty : $" (\"{key}\")")} resolved to [{(string.Join(", ", services.Select(s => s.GetType().Name)))}]");
            return services;
        }
    }
}
