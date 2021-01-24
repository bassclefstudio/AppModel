using BassClefStudio.AppModel.Bindings;
using BassClefStudio.NET.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
                    Name = "Test 1"
                }
            };
        }

        private static void TestBinding(Func<MyClass, IBinding<string>> getBinding)
        {
            var myObject = CreateTestObject();
            IBinding<string> nameBinding = getBinding(myObject);

            int val = 0;
            nameBinding.ValueChanged += (s, e) => val++;

            myObject.Property.Name = "Test 2";
            Assert.AreEqual(1, val, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, nameBinding.StoredValue, "Incorrect StoredValue on nameBinding.");

            myObject.Property = new MyPropertyClass()
            {
                Name = "Test 3"
            };
            Assert.AreEqual(2, val, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, nameBinding.StoredValue, "Incorrect StoredValue on nameBinding.");

            nameBinding.StoredValue = "Test 4";
            Assert.AreEqual("Test 4", myObject.Property.Name, "Failed to set property through binding.");
        }

        [TestMethod]
        public void TestPropertyBinding()
        {
            TestBinding(myObject => myObject.MyBinding()
                .WithProperty(m => m.Property)
                .WithProperty(p => p.Name, (p, n) => p.Name = n));
        }

        [TestMethod]
        public void TestReflectionBinding()
        {
            TestBinding(myObject => myObject.MyBinding()
                .CreateReflectionBinding<MyClass, string>("Property.Name", true));
        }

        [TestMethod]
        public void TestBadReflectionPath()
        {
            var myObject = CreateTestObject();
            Assert.ThrowsException<BindingException>(() =>
                myObject.MyBinding()
                .CreateReflectionBinding<MyClass, string>("Property.Blah"));
        }

        [TestMethod]
        public void TestGetOnlyPropertyBinding()
        {
            var myObject = CreateTestObject();
            var myBinding = myObject.MyBinding()
                .WithProperty(m => m.Property)
                .WithProperty(p => p.Name);

            Assert.ThrowsException<BindingException>(
                () => myBinding.StoredValue = "Test 2");
        }

        [TestMethod]
        public void TestGetOnlyReflectionBinding()
        {
            var myObject = CreateTestObject();
            var myBinding = myObject.MyBinding()
                .CreateReflectionBinding<MyClass, string>("Property.Name");

            Assert.ThrowsException<BindingException>(
                () => myBinding.StoredValue = "Test 2");
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
    }
}
