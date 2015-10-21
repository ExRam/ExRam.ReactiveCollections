using System.Collections.Immutable;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExRam.ReactiveCollections.Tests
{
    [TestClass]
    public class CollectionChangedNotificationTest
    {
        #region NotificationImpl
        private sealed class NotificationImpl : CollectionChangedNotification<int>
        {
            // ReSharper disable once SuggestBaseTypeForParameter
            public NotificationImpl(ImmutableList<int> current, NotifyCollectionChangedAction action, ImmutableList<int> oldItems, ImmutableList<int> newItems) : base(current, action, oldItems, newItems)
            {
            }

            public override ICollectionChangedNotification<int> ToResetNotification()
            {
                return new NotificationImpl((ImmutableList<int>)this.Current, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty);
            }
        }
        #endregion

        [TestMethod]
        public void Current_is_set()
        {
            var list = ImmutableList.Create(1, 2, 3);
            var notification = new NotificationImpl(list, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty);

            Assert.AreEqual(list, notification.Current);
        }
    }
}
