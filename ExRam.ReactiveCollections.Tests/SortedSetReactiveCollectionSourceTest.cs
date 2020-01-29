using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class SortedSetReactiveCollectionSourceTest
    {
        private struct StructNotImplementingIComparable
        {
            public StructNotImplementingIComparable(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        [Fact]
        public async Task First_notification_is_reset()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notification = await list.ReactiveCollection.Changes.FirstAsync().ToTask();

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Add()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);

            var notification = await notificationTask;

            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.Index.Should().Be(0);
            notification.NewItems.Should().Equal(1);
            notification.Current.Should().Equal(1);
        }

        [Fact]
        public async Task Add_multiple()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .Take(6)
                .Select(x => x.Index)
                .ToArray()
                .ToTask();

            list.Add(1); // 0
            list.Add(3); // 1
            list.Add(2); // 1
            list.Add(0); // 0
            list.Add(6); // 4
            list.Add(5); // 4

            var notifications = await notificationTask;

            notifications.Should().BeEquivalentTo(new[] { 0, 1, 1, 0, 4, 4 });
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Clear_preserves_comparer()
        {
            var list = new SortedSetReactiveCollectionSource<StructNotImplementingIComparable>(new Comparison<StructNotImplementingIComparable>((x, y) => x.Value.CompareTo(y.Value)).ToComparer());

            list.Add(new StructNotImplementingIComparable(1));
            list.Clear();
            list.Add(new StructNotImplementingIComparable(2));
            list.Add(new StructNotImplementingIComparable(1));

            var current = (await list.ReactiveCollection.Changes.FirstAsync()).Current.Select(x => x.Value).ToArray();

            current.Should().Equal(1, 2);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(1);
            notification.Current.Should().BeEmpty();
            notification.Index.Should().Be(0);
        }
    }
}
