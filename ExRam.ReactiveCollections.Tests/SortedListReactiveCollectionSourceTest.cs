using System;
using System.Collections;
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
    public class SortedListReactiveCollectionSourceTest
    {
        #region First_notification_is_reset
        [TestMethod]
        public async Task First_notification_is_reset()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notification = await list.ReactiveCollection.Changes.FirstAsync().ToTask();

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            Assert.AreEqual(ImmutableList<int>.Empty, notification.OldItems);
            Assert.AreEqual(ImmutableList<int>.Empty, notification.NewItems);
            Assert.AreEqual(ImmutableList<int>.Empty, notification.Current);
        }
        #endregion

        #region Add
        [TestMethod]
        public async Task Add()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 1 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1 }, notification.Current.ToArray());
        }
        #endregion

        #region AddRange
        [TestMethod]
        public async Task AddRange()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            var range = new[] { 3, 1, 2 };
            list.AddRange(range);

            var notification = await notificationsTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);
            Assert.AreEqual(0, notification.OldItems.Count);
            CollectionAssert.AreEquivalent(new[]{1,2,3}, (IList)notification.NewItems);
        }
        #endregion

        #region Clear
        [TestMethod]
        public async Task Clear()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Clear();

            var notification = await notificationTask;

            Assert.IsNull(notification.Index);

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
            var list = new SortedListReactiveCollectionSource<int>
            {
                1
            };

            Assert.IsTrue(list.Contains(1));
            Assert.IsFalse(list.Contains(2));
        }
        #endregion

        #region Contains
        [TestMethod]
        public void CopyTo()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var target = new int[5];
            list.CopyTo(target, 2);

            CollectionAssert.AreEqual(new[] { 0, 0, 1, 2, 3 }, target);
        }
        #endregion

        #region GetEnumerator
        [TestMethod]
        public void GetEnumerator()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var enumerator = list.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);

            Assert.IsFalse(enumerator.MoveNext());
        }
        #endregion

        #region IndexOf
        [TestMethod]
        public void IndexOf()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 1, 2, 3 });

            Assert.AreEqual(2, list.IndexOf(3));
            Assert.AreEqual(1, list.IndexOf(2));
            Assert.AreEqual(0, list.IndexOf(1));
        }
        #endregion

        #region Insert
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Insert()
        {
            var list = new SortedListReactiveCollectionSource<int>();
            list.Insert(0, 2);
        }
        #endregion

        #region InsertRange
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void InsertRange()
        {
            var list = new SortedListReactiveCollectionSource<int>();
            list.InsertRange(1, new[] { 1, 2, 3 });
        }
        #endregion

        #region IsReadOnly
        [TestMethod]
        public void IsReadOnly()
        {
            var list = (IList)new SortedListReactiveCollectionSource<int>();

            Assert.IsFalse(list.IsReadOnly);
        }
        #endregion

        #region Item
        [TestMethod]
        public void Item()
        {
            var list = (IList)new SortedListReactiveCollectionSource<int>();
            list.Add(1);

            Assert.AreEqual(1, list[0]);
        }
        #endregion

        #region Item
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Item_set_throws()
        {
            var list = (IList)new SortedListReactiveCollectionSource<int>();
            list.Add(0);

            list[0] = 1;
        }
        #endregion

        #region Remove
        [TestMethod]
        public async Task Remove()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Remove(1);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(1, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 1 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region RemoveAll
        [TestMethod]
        public async Task RemoveAll()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.RemoveAll(x => x % 2 == 0);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            CollectionAssert.AreEqual(new[] { 1, 3 }, notification.Current);
        }
        #endregion

        #region RemoveAt
        [TestMethod]
        public async Task RemoveAt()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(3)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.RemoveAt(1);

            var notification = await notificationTask;

            Assert.AreEqual(1, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(1, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 2 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1 }, notification.Current);
        }
        #endregion

        #region RemoveRange
        [TestMethod]
        public async Task RemoveRange()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4 });
            list.RemoveRange(new[] { 2, 4 });

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            Assert.AreEqual(0, notifications[0].NewItems.Count);
            CollectionAssert.AreEqual(new[] { 2 }, notifications[0].OldItems.ToArray());

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[1].Action);
            Assert.AreEqual(0, notifications[1].NewItems.Count);
            CollectionAssert.AreEqual(new[] { 4 }, notifications[1].OldItems.ToArray());

            CollectionAssert.AreEqual(new[] { 1, 3 }, notifications[1].Current);
        }
        #endregion

        #region RemoveRange2
        [TestMethod]
        public async Task RemoveRange2()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4 });
            list.RemoveRange(2, 2);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            CollectionAssert.AreEqual(new[] { 3, 4 }, notification.OldItems.ToArray());
            Assert.AreEqual(0, notification.NewItems.Count);

            CollectionAssert.AreEqual(new[] { 1, 2 }, notification.Current);
        }
        #endregion

        #region Replace
        [TestMethod]
        public async Task Replace()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Skip(2)
                .Take(2)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 1 });
            list.Replace(1, 5);

            var notifications = await notificationsTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notifications[0].Action);
            CollectionAssert.AreEqual(new[] { 1 }, notifications[0].OldItems.ToArray());

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notifications[1].Action);
            CollectionAssert.AreEqual(new[] { 5 }, notifications[1].NewItems.ToArray());

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, notifications[1].Current);
        }
        #endregion

        #region Reverse
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Reverse()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.Reverse();
        }
        #endregion

        #region SetItem
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void SetItem()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.SetItem(2, 6);
        }
        #endregion

        #region Sort
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Sort()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 2, 1, 3, 5, 0 });
            list.Sort();
        }
        #endregion

        #region Sort_with_comparison
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Sort_with_comparison()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 2, 1, 3, 5, 0 });
            list.Sort((x, y) => y.CompareTo(x));
        }
        #endregion
    }
}
