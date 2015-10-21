using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExRam.ReactiveCollections.Tests
{
    [TestClass]
    public class ReactiveCollectionExtensionsTest
    {
        #region Add_to_projected_list
        [TestMethod]
        public async Task Add_to_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var notifications = await notificationsTask;

            Assert.AreEqual(null, notifications[0].Index);
            Assert.AreEqual(0, notifications[1].Index);
            Assert.AreEqual(1, notifications[2].Index);
            Assert.AreEqual(2, notifications[3].Index);

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notifications[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[2].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[3].Action);

            Assert.AreEqual(0, notifications[0].NewItems.Count);

            Assert.AreEqual(1, notifications[1].NewItems.Count);
            CollectionAssert.AreEqual(new[] { "1" }, notifications[1].NewItems.ToArray());

            Assert.AreEqual(1, notifications[2].NewItems.Count);
            CollectionAssert.AreEqual(new[] { "2" }, notifications[2].NewItems.ToArray());

            Assert.AreEqual(1, notifications[3].NewItems.Count);
            CollectionAssert.AreEqual(new[] { "3" }, notifications[3].NewItems.ToArray());
        }
        #endregion

        #region Add_to_projected_set
        [TestMethod]
        public async Task Add_to_projected_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var notifications = await notificationsTask;

            Assert.AreEqual(null, notifications[0].Index);
            Assert.AreEqual(0, notifications[1].Index);
            Assert.AreEqual(1, notifications[2].Index);
            Assert.AreEqual(2, notifications[3].Index);

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notifications[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[2].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[3].Action);

            Assert.AreEqual(0, notifications[0].NewItems.Count);

            Assert.AreEqual(1, notifications[1].NewItems.Count);
            CollectionAssert.AreEqual(new[] { "1" }, notifications[1].NewItems.ToArray());

            Assert.AreEqual(1, notifications[2].NewItems.Count);
            CollectionAssert.AreEqual(new[] { "2" }, notifications[2].NewItems.ToArray());

            Assert.AreEqual(1, notifications[3].NewItems.Count);
            CollectionAssert.AreEqual(new[] { "3" }, notifications[3].NewItems.ToArray());
        }
        #endregion

        #region Add_to_projected_dictionary
        [TestMethod]
        public async Task Add_to_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notifications[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[2].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[3].Action);

            Assert.AreEqual(0, notifications[0].NewItems.Count);

            Assert.AreEqual(1, notifications[1].NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, string>("Key1", "1") }, notifications[1].NewItems.ToArray());

            Assert.AreEqual(1, notifications[2].NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, string>("Key2", "2") }, notifications[2].NewItems.ToArray());

            Assert.AreEqual(1, notifications[3].NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, string>("Key3", "3") }, notifications[3].NewItems.ToArray());

            CollectionAssert.AreEquivalent(new[] { new KeyValuePair<string, string>("Key1", "1"), new KeyValuePair<string, string>("Key2", "2"), new KeyValuePair<string, string>("Key3", "3") }, notifications[3].Current.ToArray());
        }
        #endregion

        #region Remove_from_projected_list
        [TestMethod]
        public async Task Remove_from_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Remove(2);

            var notification = await notificationTask;

            Assert.AreEqual(1, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { "2" }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { "1", "3" }, notification.Current);
        }
        #endregion

        #region Remove_from_projected_set
        [TestMethod]
        public async Task Remove_from_projected_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(4)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(2);

            var notification = await notificationTask;

            Assert.AreEqual(1, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { "2" }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { "1", "3" }, notification.Current);
        }
        #endregion

        #region Remove_from_projected_dictionary
        [TestMethod]
        public async Task Remove_from_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(4)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);
            list.Remove("Key2");

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, string>("Key2", "2") }, notification.OldItems.ToArray());
            CollectionAssert.AreEquivalent(new[] { new KeyValuePair<string, string>("Key1", "1"), new KeyValuePair<string, string>("Key3", "3") }, notification.Current);
        }
        #endregion

        #region Replace_in_projected_list
        [TestMethod]
        public async Task Replace_in_projected_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 4);

            var notification = await notificationTask;

            Assert.AreEqual(1, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { "2" }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { "4" }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { "1", "4", "3" }, notification.Current);
        }
        #endregion

        #region Replace_in_projected_from_ListChangedNotification_observable
        [TestMethod]
        public async Task Replace_in_projected_from_ListChangedNotification_observable()
        {
            var observable = new[]
                {
                    new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new ListChangedNotification<int>(ImmutableList.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new ListChangedNotification<int>(ImmutableList.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
                .ToObservable()
                .ToReactiveCollection();

            var projectedList = observable.Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            Assert.AreEqual(0, notifications[0].Index);
            Assert.AreEqual(2, notifications[0].OldItems.Count);
            Assert.AreEqual(0, notifications[0].NewItems.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(ImmutableList.Create("1", "2"), notifications[0].OldItems.ToArray());

            Assert.AreEqual(0, notifications[1].Index);
            Assert.AreEqual(0, notifications[1].OldItems.Count);
            Assert.AreEqual(2, notifications[1].NewItems.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(ImmutableList.Create("3", "4"), notifications[1].NewItems.ToArray());

            CollectionAssert.AreEqual(ImmutableList.Create("3", "4"), notifications[1].Current);
        }
        #endregion

        #region Replace_in_projected_from_SortedSetChangedNotification_observable
        [TestMethod]
        public async Task Replace_in_projected_from_SortedSetChangedNotification_observable()
        {
            var observable = new[]
                {
                    new SortedSetChangedNotification<int>(ImmutableSortedSet<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty),
                    new SortedSetChangedNotification<int>(ImmutableSortedSet.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2)),
                    new SortedSetChangedNotification<int>(ImmutableSortedSet.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4))
                }
                .ToObservable()
                .ToReactiveCollection();

            var projectedList = observable.Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            Assert.IsNull(notifications[0].Index);
            Assert.AreEqual(0, notifications[0].OldItems.Count);
            Assert.AreEqual(0, notifications[0].NewItems.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notifications[0].Action);

            Assert.AreEqual(0, notifications[1].Index);
            Assert.AreEqual(0, notifications[1].OldItems.Count);
            CollectionAssert.AreEqual(ImmutableList.Create("3", "4"), notifications[1].NewItems.ToArray());
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);

            CollectionAssert.AreEqual(ImmutableList.Create("3", "4"), notifications[1].Current);
        }
        #endregion

        #region Replace_in_projected_dictionary
        [TestMethod]
        public async Task Replace_in_projected_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Select(x => x.ToString(CultureInfo.InvariantCulture));

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);

            Assert.AreEqual(1, notifications[0].OldItems.Count);
            Assert.AreEqual(1, notifications[1].NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, string>("Key2", "2") }, notifications[0].OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, string>("Key2", "4") }, notifications[1].NewItems.ToArray());
            CollectionAssert.AreEquivalent(new[] { new KeyValuePair<string, string>("Key1", "1"), new KeyValuePair<string, string>("Key2", "4"), new KeyValuePair<string, string>("Key3", "3") }, notifications[1].Current);
        }
        #endregion

        #region Add_to_filtered_list
        [TestMethod]
        public async Task Add_to_filtered_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(3);           
            list.Add(2);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.NewItems.ToArray());
        }
        #endregion

        #region Add_to_filtered_set
        [TestMethod]
        public async Task Add_to_filtered_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(3);
            list.Add(2);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.NewItems.ToArray());
        }
        #endregion

        #region Add_to_filtered_dictionary
        [TestMethod]
        public async Task Add_to_filtered_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key3", 3);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.NewItems.ToArray());
        }
        #endregion

        #region Remove_from_filtered_list
        [TestMethod]
        public async Task Remove_from_filtered_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Remove(2);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region Remove_from_filtered_set
        [TestMethod]
        public async Task Remove_from_filtered_set()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Remove(2);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region Remove_from_filtered_dictionary
        [TestMethod]
        public async Task Remove_from_filtered_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);
            list.Remove("Key2");

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region Replace_in_filtered_list_addition
        [TestMethod]
        public async Task Replace_in_filtered_list_addition()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(1, 4);

            var notification = await notificationTask;

            Assert.AreEqual(1, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(0, notification.OldItems.Count);
            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 4 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 2, 4 }, notification.Current);
        }
        #endregion

        #region Replace_in_filtered_list_removal
        [TestMethod]
        public async Task Replace_in_filtered_list_removal()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 3);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region Replace_in_filtered_list_replacement
        [TestMethod]
        public async Task Replace_in_filtered_list_replacement()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.Replace(2, 4);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, notification.Action);

            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 4 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 4 }, notification.Current);
        }
        #endregion

        #region Replace_in_filtered_list_replacement2
        [TestMethod]
        public async Task Replace_in_filtered_list_replacement2()
        {
            var list = new ListReactiveCollectionSource<int>(new[] { 1 });

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Replace(1, 3);
            list.Replace(3, 4);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            CollectionAssert.AreEqual(new int[0], notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 4 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 4 }, notification.Current);
        }
        #endregion

        #region Replace_in_filtered_dictionary_addition
        [TestMethod]
        public async Task Replace_in_filtered_dictionary_addition()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key1"] = 4;

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(0, notification.OldItems.Count);
            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 4) }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key1", 4) }, notification.Current);
        }
        #endregion

        #region Replace_in_filtered_dictionary_removal
        [TestMethod]
        public async Task Replace_in_filtered_dictionary_removal()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 3;

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.OldItems.ToArray());
            Assert.AreEqual(0, notification.NewItems.Count);
        }
        #endregion

        #region Replace_in_filtered_dictionary_replacement
        [TestMethod]
        public async Task Replace_in_filtered_dictionary_replacement()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notifications[0].OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 4) }, notifications[1].NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 4) }, notifications[1].Current);
        }
        #endregion

        #region Replace_in_filtered_from_ListChangedNotification_observable
        [TestMethod]
        public async Task Replace_in_filtered_from_ListChangedNotification_observable()
        {
            var observable = new[]
                {
                    new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new ListChangedNotification<int>(ImmutableList.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new ListChangedNotification<int>(ImmutableList.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
            .ToObservable()
            .ToReactiveCollection();

            var projectedList = observable.Where(x => x % 2 == 0);

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            Assert.AreEqual(0, notifications[0].Index);
            Assert.AreEqual(0, notifications[0].NewItems.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(ImmutableList.Create(2), notifications[0].OldItems.ToArray());

            Assert.AreEqual(0, notifications[1].Index);
            Assert.AreEqual(0, notifications[1].OldItems.Count);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(ImmutableList.Create(4), notifications[1].NewItems.ToArray());

            CollectionAssert.AreEqual(ImmutableList.Create(4), notifications[1].Current);
        }
        #endregion

        #region Add_to_sorted_list
        [TestMethod]
        public async Task Add_to_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add(3);
            list.Add(1);
            list.Add(2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, notification.Current);
        }
        #endregion

        #region Add_to_sorted_dictionary
        [TestMethod]
        public async Task Add_to_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2)  }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key3", 3) }, notification.Current);
        }
        #endregion

        #region Add_to_setSorted_dictionary
        [TestMethod]
        public async Task Add_to_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key3", 3) }, notification.Current);
        }
        #endregion

        #region Add_to_setSorted_dictionary_with_duplicate_value
        [TestMethod]
        public async Task Add_to_setSorted_dictionary_with_duplicate_value()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list
                .ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key3", 3);
            list.Add("Key1", 1);
            list.Add("Key1", 1);
            list.Add("Key2", 2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key2", 2), new KeyValuePair<string, int>("Key3", 3) }, notification.Current);
        }
        #endregion

        #region Add_to_sorted_list_with_Changes
        [TestMethod]
        public async Task Add_to_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add(3);
            list.Add(1);
            list.Add(2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, notification.Current);
        }
        #endregion

        #region Remove_from_sorted_list
        [TestMethod]
        public async Task Remove_from_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Remove(2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 3 }, notification.Current);
        }
        #endregion

        #region Remove_in_sorted_dictionary
        [TestMethod]
        public async Task Remove_in_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Remove("Key1");

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1) }, notification.OldItems.ToArray());
            Assert.AreEqual(0, notification.Index);

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.Current);
        }
        #endregion

        #region Remove_in_setSorted_dictionary
        [TestMethod]
        public async Task Remove_in_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationTask = projectedList.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Remove("Key1");

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1) }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notification.Current);
        }
        #endregion

        #region Remove_from_sorted_list_with_Changes
        [TestMethod]
        public async Task Remove_from_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationTask = projectedList.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Remove(2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);

            Assert.AreEqual(1, notification.OldItems.Count);
            Assert.AreEqual(0, notification.NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 3 }, notification.Current);
        }
        #endregion

        #region Replace_in_sorted_list
        [TestMethod]
        public async Task Replace_in_sorted_list()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(new[] { 2 }, notifications[0].OldItems.ToArray());

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(new[] { 4 }, notifications[1].NewItems.ToArray());

            CollectionAssert.AreEqual(new[] { 1, 3 }, notifications[0].Current);
            CollectionAssert.AreEqual(new[] { 1, 3, 4 }, notifications[1].Current);
        }
        #endregion

        #region Replace_in_sorted_list_with_Changes
        [TestMethod]
        public async Task Replace_in_sorted_list_with_Changes()
        {
            var list = new ListReactiveCollectionSource<int>();

            var projectedList = list
                .ReactiveCollection
                .Sort();

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 2, 1 });
            list.Replace(2, 4);

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(new[] { 2 }, notifications[0].OldItems.ToArray());

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(new[] { 4 }, notifications[1].NewItems.ToArray());

            CollectionAssert.AreEqual(new[] { 1, 3 }, notifications[0].Current);
            CollectionAssert.AreEqual(new[] { 1, 3, 4 }, notifications[1].Current);
        }
        #endregion

        #region Replace_in_sorted_from_ListChangedNotification_observable
        [TestMethod]
        public async Task Replace_in_sorted_from_ListChangedNotification_observable()
        {
            var observable = new[]
                {
                    new ListChangedNotification<int>(ImmutableList<int>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty, null),
                    new ListChangedNotification<int>(ImmutableList.Create(1, 2), NotifyCollectionChangedAction.Add, ImmutableList<int>.Empty, ImmutableList.Create(1, 2), 0),
                    new ListChangedNotification<int>(ImmutableList.Create(3, 4), NotifyCollectionChangedAction.Replace, ImmutableList.Create(1, 2), ImmutableList.Create(3, 4), 0)
                }
                .ToObservable()
                .ToReactiveCollection();

            var projectedList = observable.Sort();

            var notificationsTask = projectedList.Changes
                .Skip(2)
                .Take(4)
                .ToArray()
                .ToTask();

            var notifications = await notificationsTask;

            Assert.AreEqual(0, notifications[0].Index);
            Assert.AreEqual(0, notifications[0].NewItems.Count);
            CollectionAssert.AreEqual(new[] { 1 }, notifications[0].OldItems.ToArray());
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);

            Assert.AreEqual(0, notifications[1].Index);
            Assert.AreEqual(0, notifications[1].NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notifications[1].OldItems.ToArray());
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[1].Action);

            Assert.AreEqual(0, notifications[2].Index);
            Assert.AreEqual(0, notifications[2].OldItems.Count);
            CollectionAssert.AreEqual(new[] { 3 }, notifications[2].NewItems.ToArray());
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[2].Action);

            Assert.AreEqual(1, notifications[3].Index);
            Assert.AreEqual(0, notifications[3].OldItems.Count);
            CollectionAssert.AreEqual(new[] { 4 }, notifications[3].NewItems.ToArray());
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[3].Action);

            CollectionAssert.AreEqual(ImmutableList.Create(3, 4), notifications[3].Current);
        }
        #endregion

        #region Replace_in_sorted_dictionary
        [TestMethod]
        public async Task Replace_in_sorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .Sort(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notifications[0].OldItems.ToArray());
            Assert.AreEqual(1, notifications[0].Index);

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 4) }, notifications[1].NewItems.ToArray());
            Assert.AreEqual(2, notifications[1].Index);

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key3", 3), new KeyValuePair<string, int>("Key2", 4) }, notifications[1].Current);
        }
        #endregion

        #region Replace_in_setSorted_dictionary
        [TestMethod]
        public async Task Replace_in_setSorted_dictionary()
        {
            var list = new DictionaryReactiveCollectionSource<string, int>();

            var projectedList = list.ReactiveCollection
                .SortSet(Comparer<KeyValuePair<string, int>>.Create((x, y) => x.Value.CompareTo(y.Value)));

            var notificationsTask = projectedList.Changes
                .Skip(4)
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add("Key1", 1);
            list.Add("Key2", 2);
            list.Add("Key3", 3);

            list["Key2"] = 4;

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 2) }, notifications[0].OldItems.ToArray());

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key2", 4) }, notifications[1].NewItems.ToArray());

            CollectionAssert.AreEqual(new[] { new KeyValuePair<string, int>("Key1", 1), new KeyValuePair<string, int>("Key3", 3), new KeyValuePair<string, int>("Key2", 4) }, notifications[1].Current);
        }
        #endregion

        #region ToObservableCollection_Add
        [TestMethod]
        public async Task ToObservableCollection_Add()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = ((INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection());

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            var events = await eventsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[2].Action);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, (ICollection)observableCollection);
        }
        #endregion

        #region ToObservableCollection_Add_multiple
        [TestMethod]
        public void ToObservableCollection_Add_multiple()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = list.ReactiveCollection.ToObservableCollection();

            using (Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged += eh, eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged -= eh)
                .Subscribe())
            {
                list.AddRange(new[] { 1, 2, 3 });
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, (ICollection)observableCollection);

                list.InsertRange(2, new[] { 4, 5, 6 });
                CollectionAssert.AreEqual(new[] { 1, 2, 4, 5, 6, 3 }, (ICollection)observableCollection);
            }
        }
        #endregion

        #region ToObservableCollection_Remove
        [TestMethod]
        public async Task ToObservableCollection_Remove()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = ((INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection());

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Remove(1);

            var events = await eventsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, events[2].Action);
        }
        #endregion

        #region ToObservableCollection_Remove_multiple
        [TestMethod]
        public void ToObservableCollection_Remove_multiple()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = list.ReactiveCollection.ToObservableCollection();

            using (Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged += eh, eh => ((INotifyCollectionChanged)observableCollection).CollectionChanged -= eh)
                .Subscribe())
            {
                list.AddRange(new[] { 1, 2, 3, 4, 5, 6, 7 });
                list.RemoveRange(2, 3);

                CollectionAssert.AreEqual(new[] { 1, 2, 6, 7 }, (ICollection)observableCollection);
            }
        }
        #endregion

        #region ToObservableCollection_Replace
        [TestMethod]
        public async Task ToObservableCollection_Replace()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = ((INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection());

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Take(3)
                .Select(x => x.EventArgs)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.Replace(1, 3);

            var events = await eventsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[0].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, events[1].Action);
            Assert.AreEqual(NotifyCollectionChangedAction.Replace, events[2].Action);

            CollectionAssert.AreEqual(new[] { 1 }, events[2].OldItems);
            CollectionAssert.AreEqual(new[] { 3 }, events[2].NewItems);

            CollectionAssert.AreEqual(new[] { 3, 2 }, (ICollection)observableCollection);
        }
        #endregion

        #region ToObservableCollection_Clear
        [TestMethod]
        public async Task ToObservableCollection_Clear()
        {
            var list = new ListReactiveCollectionSource<int>();
            var observableCollection = ((INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection());

            var eventTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Skip(1)
                .Select(x => x.EventArgs)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.RemoveAll(x => x > 2);

            var ev = await eventTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, ev.Action);
            CollectionAssert.AreEqual(new[] { 1, 2 }, (ICollection)observableCollection);
        }
        #endregion

        #region ToObservableCollection_does_not_raise_events_on_event_subscription
        [TestMethod]
        public async Task ToObservableCollection_does_not_raise_events_on_event_subscription()
        {
            var list = new ListReactiveCollectionSource<int> {1, 2, 3};

            var observableCollection = ((INotifyCollectionChanged)list.ReactiveCollection.ToObservableCollection());

            var eventsTask = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(eh => observableCollection.CollectionChanged += eh, eh => observableCollection.CollectionChanged -= eh)
                .Select(x => x.EventArgs)
                .FirstAsync()
                .ToTask();

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, (ICollection)observableCollection);
            list.Add(4);

            var ev = await eventsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, ev.Action);
            CollectionAssert.AreEqual(new[] { 4 }, ev.NewItems);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, (ICollection)observableCollection);
        }
        #endregion

        #region GetValueObservable_Test1
        [TestMethod]
        public async Task GetValueObservable_Test1()
        {
            var dict = new DictionaryReactiveCollectionSource<int, int>();

            var arrayTask = dict.ReactiveCollection.GetValueObservable(1)
                .Take(4)
                .ToArray()
                .ToTask();

            dict.Add(2, 2);
            dict.Add(1, 1);
            dict[2] = 1;
            dict.Remove(1);
            dict[2] = 1;
            dict.Add(1, 2);
            dict[1] = 3;

            CollectionAssert.AreEqual(new[] { 1, 1, 2, 3 }, await arrayTask);
        }
        #endregion
    }
}
