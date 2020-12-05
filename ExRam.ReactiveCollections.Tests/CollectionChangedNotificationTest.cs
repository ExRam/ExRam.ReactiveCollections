using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class CollectionChangedNotificationTest : VerifyBase
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
                return new NotificationImpl((ImmutableList<int>)Current, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty);
            }
        }
        #endregion

        public CollectionChangedNotificationTest() : base()
        {

        }
        
        [Fact]
        public async Task Current_is_set()
        {
            var list = ImmutableList.Create(1, 2, 3);
            var notification = new NotificationImpl(list, NotifyCollectionChangedAction.Reset, ImmutableList<int>.Empty, ImmutableList<int>.Empty);

            await Verify(notification.Current);
        }
    }
}
