using System;
using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class ListReactiveCollectionSourceTest : VerifyBase
    {
        public ListReactiveCollectionSourceTest() : base()
        {
            
        }

        [Fact]
        public async Task First_notification_is_reset()
        {
            var list = new ListReactiveCollectionSource<int>();

            await Verify(list.ReactiveCollection.Changes.FirstAsync().ToTask());
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
        }

        [Fact]
        public async Task Contains()
        {
            var list = new ListReactiveCollectionSource<int>
            {
                1
            };

            await Verify((
                list.Contains(1),
                list.Contains(2)));
        }

        [Fact]
        public async Task CopyTo()
        {
            var list = new ListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var target = new int[5];
            list.CopyTo(target, 2);

            await Verify(target);
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
        public async Task IndexOf()
        {
            var list = new ListReactiveCollectionSource<int>
            {
                1, 2, 3
            };

            await Verify((
                list.IndexOf(3),
                list.IndexOf(2),
                list.IndexOf(1)));
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
        }

        [Fact]
        public async Task IsReadOnly()
        {
            var list = (IList)new ListReactiveCollectionSource<int>();

            await Verify(list.IsReadOnly);
        }

        [Fact]
        public void Item()
        {
            var list = (IList)new ListReactiveCollectionSource<int>();
            
            list
                .Invoking(_ => _.Add(1))
                .Should()
                .Throw<NotSupportedException>();
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
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

            await Verify(notificationTask);
        }
    }
}
