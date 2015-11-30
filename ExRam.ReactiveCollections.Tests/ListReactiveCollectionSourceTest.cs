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
    public class ListReactiveCollectionSourceTest
    {
        #region First_notification_is_reset
        [TestMethod]
        public async Task First_notification_is_reset()
        {
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var notification = await notificationTask;

            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(3, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(range, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(range, notification.Current);
        }
        #endregion

        #region Clear
        [TestMethod]
        public async Task Clear()
        {
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>
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
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

            list.AddRange(new[] { 1, 2, 3 });

            Assert.AreEqual(2, list.IndexOf(3));
            Assert.AreEqual(1, list.IndexOf(2));
            Assert.AreEqual(0, list.IndexOf(1));
        }
        #endregion

        #region Insert
        [TestMethod]
        public async Task Insert()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Insert(0, 2);

            var notification = await notificationTask;
            
            Assert.AreEqual(0, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 2 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 2 }, notification.Current);
        }
        #endregion

        #region InsertRange
        [TestMethod]
        public async Task InsertRange()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);

            var range = new[] { 1, 2, 3 };
            list.InsertRange(1, range);

            var notification = await notificationTask;

            Assert.AreEqual(1, notification.Index);
            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);
            Assert.AreEqual(3, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(range, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 1, 2, 3 }, notification.Current);
        }
        #endregion

        #region IsReadOnly
        [TestMethod]
        public void IsReadOnly()
        {
            var list = (IList)new ListReactiveCollectionSource<int>();

            Assert.IsFalse(list.IsReadOnly);
        }
        #endregion

        #region Item
        [TestMethod]
        public void Item()
        {
            var list = (IList)new ListReactiveCollectionSource<int>();
            list.Add(1);

            Assert.AreEqual(1, list[0]);
        }
        #endregion

        #region Remove
        [TestMethod]
        public async Task Remove()
        {
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4 });
            list.RemoveRange(new[] { 2, 4 });

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 1, 3 }, notification.Current);
        }
        #endregion

        #region RemoveRange2
        [TestMethod]
        public async Task RemoveRange2()
        {
            var list = new ListReactiveCollectionSource<int>();

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
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 1 });
            list.Replace(1, 5);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, notification.Action);
            CollectionAssert.AreEqual(new[] { 1 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 5 }, notification.NewItems.ToArray());

            CollectionAssert.AreEqual(new[] { 5, 2, 3, 4, 1 }, notification.Current);
        }
        #endregion

        #region Reverse
        [TestMethod]
        public async Task Reverse()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.Reverse();

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1 }, notification.Current);
        }
        #endregion

        #region SetItem
        [TestMethod]
        public async Task SetItem()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 5 });
            list.SetItem(2, 6);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Replace, notification.Action);
            Assert.AreEqual(2, notification.Index);

            CollectionAssert.AreEqual(new[] { 3 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new[] { 6 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 2, 6, 4, 5 }, notification.Current);
        }
        #endregion

        #region Sort
        [TestMethod]
        public async Task Sort()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 2, 1, 3, 5, 0 });
            list.Sort();

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 5 }, notification.Current);
        }
        #endregion

        #region Sort_with_comparison
        [TestMethod]
        public async Task Sort_with_comparison()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.AddRange(new[] { 2, 1, 3, 5, 0 });
            list.Sort((x, y) => y.CompareTo(x));

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            CollectionAssert.AreEqual(new[] { 5, 3, 2, 1, 0 }, notification.Current);
        }
        #endregion
    }
}
