using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExRam.ReactiveCollections.Tests
{
    [TestClass]
    public class DictionaryReactiveCollectionSourceTest
    {
        #region First_notification_is_reset
        [TestMethod]
        public async Task First_notification_is_reset()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notification = await list.ReactiveCollection.Changes.FirstAsync().ToTask();

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            Assert.AreEqual(ImmutableList<KeyValuePair<string, int>>.Empty, notification.OldItems);
            Assert.AreEqual(ImmutableList<KeyValuePair<string, int>>.Empty, notification.NewItems);
            Assert.AreEqual(ImmutableDictionary<string, int>.Empty, notification.Current);
        }
        #endregion

        #region Add
        [TestMethod]
        public async Task Add()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add("Key", 1);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key", 1) }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key", 1) }, notification.Current);
        }
        #endregion

        #region Add
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Adding_existing_key_throws()
        {
            // ReSharper disable ObjectCreationAsStatement
            new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key", 1 },
                { "Key", 2 }
            };
            // ReSharper restore ObjectCreationAsStatement
        }

        #endregion

        #region AddRange
        [TestMethod]
        public async Task AddRange()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            var range = new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 2),
                new KeyValuePair<string, int>("Key3", 3)
            };

            list.AddRange(range);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(3, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(range, notification.NewItems.ToArray());
            CollectionAssert.AreEquivalent(range, notification.Current);
        }
        #endregion

        #region Clear
        [TestMethod]
        public async Task Clear()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key", 1);
            list.Clear();

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);

            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region Contains
        [TestMethod]
        public void Contains()
        {
            var dict = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key", 1 }
            };

            Assert.IsTrue(dict.Contains(new KeyValuePair<string, int>("Key", 1)));
            Assert.IsFalse(dict.Contains(new KeyValuePair<string, int>("Key", 2)));
            Assert.IsTrue(dict.ContainsKey("Key"));
            Assert.IsFalse(dict.ContainsKey("Key1"));
        }
        #endregion

        #region Remove
        [TestMethod]
        public async Task Remove()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Remove("Key1");

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(1, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1) }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new KeyValuePair<string, int>[0], notification.Current);
        }
        #endregion

        #region RemoveRange
        [TestMethod]
        public async Task RemoveRange()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 2),
                new KeyValuePair<string, int>("Key3", 3)
            });

            list.RemoveRange(new[]
            {
                "Key1",
                "Key2"
            });

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1) }, notifications[0].OldItems.ToArray());

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[1].Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notifications[1].OldItems.ToArray());

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key3", 3) }, notifications[1].Current);
        }
        #endregion

        #region SetItem
        [TestMethod]
        public async Task SetItem()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            var range = new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 2),
                new KeyValuePair<string, int>("Key3", 3)
            };

            list.AddRange(range);

            list.SetItem("Key2", 3);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, notification.Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 3) }, notification.NewItems.ToArray());

            CollectionAssert.AreEquivalent(new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 3),
                new KeyValuePair<string, int>("Key3", 3)
            }, notification.Current);
        }
        #endregion

        #region SetItems
        [TestMethod]
        public async Task SetItems()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(4)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[]
            {
                new KeyValuePair<string, int>("Key1", 1),
                new KeyValuePair<string, int>("Key2", 2),
                new KeyValuePair<string, int>("Key3", 3)
            });

            list.SetItems(new[]
            {
                new KeyValuePair<string, int>("Key1", 4),
                new KeyValuePair<string, int>("Key2", 5),
                new KeyValuePair<string, int>("Key3", 6)
            });

            var notification = await notificationTask;

            CollectionAssert.AreEquivalent(new[]
            {
                new KeyValuePair<string, int>("Key1", 4),
                new KeyValuePair<string, int>("Key2", 5),
                new KeyValuePair<string, int>("Key3", 6)
            }, notification.Current);
        }
        #endregion

        #region TryGetValue
        [TestMethod]
        public void TryGetValue()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
                {
                { "Key1", 1 }
            };

            int value;
            Assert.IsTrue(list.TryGetValue("Key1", out value));
            Assert.AreEqual(1, value);

            Assert.IsFalse(list.TryGetValue("Key2", out value));
        }
        #endregion

        #region Item
        [TestMethod]
        public void Item()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
                {
                { "Key1", 1 }
            };

            Assert.AreEqual(1, list["Key1"]);

            list["Key1"] = 2;
            Assert.AreEqual(2, list["Key1"]);
        }
        #endregion

        #region Item2
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Item2()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            Assert.AreEqual(1, list["Key"]);
        }
        #endregion

        #region GetEnumerator
        [TestMethod]
        public void GetEnumerator()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>
            {
                { "Key1", 1 }
            };

            var enumerator = list.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(new KeyValuePair<string, int>("Key1", 1), enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
        }
        #endregion
    }
}
