using System.Collections;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class ListReactiveCollectionSourceTest
    {
        [Fact]
        public async Task First_notification_is_reset()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notification = await list.ReactiveCollection.Changes.FirstAsync().ToTask();

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public async Task Add()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Add(1);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(1);
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().Equal(1);
        }

        [Fact]
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

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.OldItems.Should().BeEmpty();
            notification.NewItems.Should().Equal(range);
            notification.Current.Should().Equal(range);
        }

        [Fact]
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

            notification.Index.Should().NotHaveValue();
            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().BeEmpty();
        }

        [Fact]
        public void Contains()
        {
            var list = new ListReactiveCollectionSource<int>
            {
                1
            };

            list.Contains(1).Should().BeTrue();
            list.Contains(2).Should().BeFalse();
        }

        [Fact]
        public void CopyTo()
        {
            var list = new ListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var target = new int[5];
            list.CopyTo(target, 2);

            target.Should().Equal(0, 0, 1, 2, 3);
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new ListReactiveCollectionSource<int>
            {
                1,
                2,
                3
            };

            using (var enumerator = list.GetEnumerator())
            {
                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Should().Be(1);

                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Should().Be(2);

                enumerator.MoveNext().Should().BeTrue();
                enumerator.Current.Should().Be(3);

                enumerator.MoveNext().Should().BeFalse();
            }
        }

        [Fact]
        public void IndexOf()
        {
            var list = new ListReactiveCollectionSource<int>
            {
                1, 2, 3
            };

            list.IndexOf(3).Should().Be(2);
            list.IndexOf(2).Should().Be(1);
            list.IndexOf(1).Should().Be(0);
        }

        [Fact]
        public async Task Insert()
        {
            var list = new ListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Skip(1)
                .FirstAsync()
                .ToTask();

            list.Insert(0, 2);

            var notification = await notificationTask;

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(2);
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().Equal(2);
        }

        [Fact]
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

            notification.Index.Should().Be(1);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Add);
            notification.NewItems.Should().Equal(1, 2, 3);
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().Equal(1, 1, 2, 3);
        }

        [Fact]
        public void IsReadOnly()
        {
            var list = (IList)new ListReactiveCollectionSource<int>();

            list.IsReadOnly.Should().BeFalse();
        }

        [Fact]
        public void Item()
        {
            var list = (IList)new ListReactiveCollectionSource<int>();
            list.Add(1);

            list[0].Should().Be(1);
        }

        [Fact]
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

            notification.Index.Should().Be(0);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(1);
            notification.Current.Should().BeEmpty();
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(1, 3);
        }

        [Fact]
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

            notification.Index.Should().Be(1);
            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().Equal(2);
            notification.Current.Should().Equal(1);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.NewItems.Should().BeEmpty();
            notification.OldItems.Should().BeEmpty();
            notification.Current.Should().Equal(1, 3);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Remove);
            notification.OldItems.Should().ContainInOrder(3, 4);
            notification.NewItems.Should().BeEmpty();
            notification.Current.Should().Equal(1, 2);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notification.OldItems.Should().Equal(1);
            notification.NewItems.Should().Equal(5);
            notification.Current.Should().Equal(5, 2, 3, 4, 1);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(5, 4, 3, 2, 1);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Replace);
            notification.Index.Should().Be(2);
            notification.OldItems.Should().Equal(3);
            notification.NewItems.Should().Equal(6);
            notification.Current.Should().Equal(1, 2, 6, 4, 5);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(0, 1, 2, 3, 5);
        }

        [Fact]
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

            notification.Action.Should().Be(NotifyCollectionChangedAction.Reset);
            notification.Current.Should().Equal(5, 3, 2, 1, 0);
        }
    }
}
