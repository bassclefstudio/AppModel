﻿using BassClefStudio.AppModel.Streams;
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
    public class StreamTests
    {
        #region Binding

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

        private static async Task TestBinding(Func<MyClass, IStream<string>> getBinding)
        {
            var myObject = CreateTestObject();
            IStream<string> nameBinding = getBinding(myObject);

            string value = null;
            nameBinding.ValueEmitted += (s, e) => value = e.Result;
            await nameBinding.StartAsync();

            myObject.Property.Name = "Test 2";
            Assert.IsNotNull(value, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, value, "Incorrect StoredValue on nameBinding.");

            value = null;
            myObject.Property = new MyPropertyClass()
            {
                Name = "Test 3"
            };
            Assert.IsNotNull(value, "ValueChanged event was not fired.");
            Assert.AreEqual(myObject.Property.Name, value, "Incorrect StoredValue on nameBinding.");
        }

        [TestMethod]
        public async Task TestPropertyBinding()
        {
            await TestBinding(myObject => myObject.AsStream()
                .Property(m => m.Property)
                .Property(p => p.Name));
            //// Strongly-typed - think {x:Bind Property.Name}
        }

        [TestMethod]
        public async Task TestReflectionBinding()
        {
            await TestBinding(myObject => myObject.AsStream()
                .Property<MyClass, string>("Property.Name"));
            //// Reflection - weakly-typed - think {Binding Property.Name}
        }

        [TestMethod]
        public void TestBadReflectionPath()
        {
            var myObject = CreateTestObject();
            Assert.ThrowsException<StreamException>(() =>
                myObject.AsStream()
                .Property<MyClass, string>("Property.Blah"));
            //// Reflection - weakly-typed - think {Binding Property.Blah} (where the property name doesn't exist).
        }

        #endregion
        #region Linq

        [TestMethod]
        public async Task TestFilter()
        {
            string[] values = new string[] { "wow!", "hello", "cool!", "great", "awesome!" };
            List<string> results = new List<string>();
            var stream = new SourceStream<string>(values)
                .Where(s => s.Last() == '!')
                .BindResult(results.Add);
            await stream.StartAsync();
            Assert.AreEqual(3, results.Count, "Result does not contain expected number of items.");
            Assert.IsTrue(results.SequenceEqual(new string[] { values[0], values[2], values[4] }));
        }

        [TestMethod]
        public async Task TestAggregateCounter()
        {
            int length = 8;
            int number = 0;
            var stream = SourceStream<string>.Repeat("Hello World!", length)
                .Aggregate<string, int>((n, s) => n + 1)
                .BindResult(n => number = n);
            await stream.StartAsync();
            Assert.AreEqual(length, number, "Aggregate was not expected value.");
        }

        [TestMethod]
        public async Task TestSum()
        {
            int length = 8;
            int number = 0;
            var stream = SourceStream<int>.Repeat(2, length)
                .Sum()
                .BindResult(n => number = n);
            await stream.StartAsync();
            Assert.AreEqual(length * 2, number, "Sum was not expected value.");
        }

        #endregion
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