using BassClefStudio.AppModel.Bindings;
using BassClefStudio.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Tests
{
    [TestClass]
    public class BindingTests
    {
        private static MyClass CreateTestObject()
        {
            return new MyClass()
            {
                Property = new MyPropertyClass()
                {
                    Name = "Test 1",
                    Keys = new ObservableCollection<string>() { "Test 1" }
                }
            };
        }

        private static void TestBinding(Func<MyClass, IBinding<string>> getBinding)
        {
            var myObject = CreateTestObject();
            IBinding<string> nameBinding = getBinding(myObject);

            int val = 0;
            nameBinding.CurrentValueChanged += (s, e) => val++;

            myObject.Property.Name = "Test 2";
            Assert.AreEqual(1, val, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, nameBinding.CurrentValue, "Incorrect StoredValue on nameBinding.");

            myObject.Property = new MyPropertyClass()
            {
                Name = "Test 3"
            };
            Assert.AreEqual(2, val, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, nameBinding.CurrentValue, "Incorrect StoredValue on nameBinding.");

            nameBinding.CurrentValue = "Test 4";
            Assert.AreEqual("Test 4", myObject.Property.Name, "Failed to set property through binding.");
        }

        private static void TestCollectionBinding(Func<MyClass, IBinding<ObservableCollection<string>>> getBinding)
        {
            var myObject = CreateTestObject();
            IBinding<ObservableCollection<string>> collectionBinding = getBinding(myObject);

            int val = 0;
            collectionBinding.CurrentValueChanged += (s, e) => val++;

            Assert.AreEqual(myObject.Property.Keys[0], collectionBinding.CurrentValue[0], "Incorrect initial StoredValue on collectionBinding.");
            myObject.Property.Keys.Add("Test 2");
            Assert.AreEqual(1, val, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Keys[1], collectionBinding.CurrentValue[1], "Incorrect StoredValue on collectionBinding.");

            myObject.Property = new MyPropertyClass()
            {
                Keys = new ObservableCollection<string>() { "Test 3" }
            };
            Assert.AreEqual(2, val, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Keys[0], collectionBinding.CurrentValue[0], "Incorrect StoredValue on collectionBinding.");
        }

        [TestMethod]
        public void TestPropertyBinding()
        {
            TestBinding(myObject => myObject.MyBinding()
                .Property(m => m.Property)
                .Property(p => p.Name, (p, n) => p.Name = n));
            //// Strongly-typed - think {x:Bind Property.Name}
        }

        [TestMethod]
        public void TestPropertyCollectionBinding()
        {
            TestCollectionBinding(myObject => myObject.MyBinding()
                .Property(m => m.Property)
                .Property(p => p.Keys)
                .AsCollection());
            //// Strongly-typed - think {x:Bind Property.Keys}
        }

        [TestMethod]
        public void TestReflectionBinding()
        {
            TestBinding(myObject => myObject.MyBinding()
                .ReflectionBinding<MyClass, string>("Property.Name", true));
            //// Reflection - weakly-typed - think {Binding Property.Name}
        }

        [TestMethod]
        public void TestReflectionCollectionBinding()
        {
            TestCollectionBinding(myObject => myObject.MyBinding()
                .ReflectionBinding<MyClass, ObservableCollection<string>>("Property.Keys")
                .AsCollection());
            //// Reflection - weakly-typed - think {Binding Property.Keys}
        }



        [TestMethod]
        public void TestBadReflectionPath()
        {
            var myObject = CreateTestObject();
            Assert.ThrowsException<BindingException>(() =>
                myObject.MyBinding()
                .ReflectionBinding<MyClass, string>("Property.Blah"));
        }

        [TestMethod]
        public void TestGetOnlyPropertyBinding()
        {
            var myObject = CreateTestObject();
            var myBinding = myObject.MyBinding()
                .Property(m => m.Property)
                .Property(p => p.Name);

            Assert.ThrowsException<BindingException>(
                () => myBinding.CurrentValue = "Test 2");
        }

        [TestMethod]
        public void TestGetOnlyReflectionBinding()
        {
            var myObject = CreateTestObject();
            var myBinding = myObject.MyBinding()
                .ReflectionBinding<MyClass, string>("Property.Name");

            Assert.ThrowsException<BindingException>(
                () => myBinding.CurrentValue = "Test 2");
        }
    }

    class MyClass : Observable
    {
        private MyPropertyClass property;
        public MyPropertyClass Property { get => property; set => Set(ref property, value); }
    }

    class MyPropertyClass : Observable
    {
        private string name;
        public string Name { get => name; set => Set(ref name, value); }

        public ObservableCollection<string> Keys { get; set; }
    }
}
