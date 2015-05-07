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
    public class SortedSetReactiveCollectionSourceTest
    {
        #region First_notification_is_reset
        [TestMethod]
        public async Task First_notification_is_reset()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notification = await list.ReactiveCollection.Changes.FirstAsync().ToTask();

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);
            Assert.AreEqual(ImmutableList<int>.Empty, notification.OldItems);
            Assert.AreEqual(ImmutableList<int>.Empty, notification.NewItems);
            Assert.AreEqual(ImmutableSortedSet<int>.Empty, notification.Current);
        }
        #endregion

        #region Add
        [TestMethod]
        public async Task Add()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Add, notification.Action);

            Assert.AreEqual(1, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 1 }, notification.NewItems.ToArray());
            CollectionAssert.AreEqual(new[] { 1 }, notification.Current.ToArray());
        }
        #endregion

        #region Clear
        [TestMethod]
        public async Task Clear()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Clear();

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Reset, notification.Action);

            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(0, notification.OldItems.Count);
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion

        #region Remove
        [TestMethod]
        public async Task Remove()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(2)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Remove(1);

            var notification = await notificationTask;

            Assert.AreEqual(NotifyCollectionChangedAction.Remove, notification.Action);
            Assert.AreEqual(0, notification.NewItems.Count);
            Assert.AreEqual(1, notification.OldItems.Count);

            CollectionAssert.AreEqual(new[] { 1 }, notification.OldItems.ToArray());
            CollectionAssert.AreEqual(new int[0], notification.Current);
        }
        #endregion
    }
}
