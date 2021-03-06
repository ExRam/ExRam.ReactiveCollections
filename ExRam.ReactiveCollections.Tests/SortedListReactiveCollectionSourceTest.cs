﻿using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyXunit;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class SortedListReactiveCollectionSourceTest : VerifyBase
    {
        public SortedListReactiveCollectionSourceTest() : base()
        {

        }

        [Fact]
        public async Task First_notification_is_reset()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = await list.ReactiveCollection.Changes
                .Take(1)
                .ToArray()
                .ToTask();

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add(1);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task AddRange()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(4)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 3, 1, 2 });

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Clear()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Clear();

            await Verify(notificationsTask);
        }

        [Fact]
        public void Contains()
        {
            var list = new SortedListReactiveCollectionSource<int>
            {
                1
            };

            list.Contains(1).Should().BeTrue();
            list.Contains(2).Should().BeFalse();
        }

        [Fact]
        public void CopyTo()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var target = new int[5];
            list.CopyTo(target, 2);

            target.Should().Equal(0, 0, 1, 2, 3);
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

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
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 1, 2, 3 });

            list.IndexOf(3).Should().Be(2);
            list.IndexOf(2).Should().Be(1);
            list.IndexOf(1).Should().Be(0);
        }

        [Fact]
        public void Item()
        {
            var list = new SortedListReactiveCollectionSource<int>();
            list.Add(1);

            list[0].Should().Be(1);
        }

        [Fact]
        public async Task Remove()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Remove(1);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task RemoveAll()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(5)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3 });
            list.RemoveAll(x => x % 2 == 0);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task RemoveAt()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(4)
                .ToArray()
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Add(2);
            list.RemoveAt(1);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task RemoveRange()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4 });
            list.RemoveRange(new[] { 2, 4 });

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task RemoveRange2()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationTask = list.ReactiveCollection.Changes
                .Take(6)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4 });
            list.RemoveRange(2, 2);

            await Verify(notificationTask);
        }

        [Fact]
        public async Task Replace()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(8)
                .ToArray()
                .ToTask();

            list.AddRange(new[] { 1, 2, 3, 4, 1 });
            list.Replace(1, 5);

            await Verify(notificationsTask);
        }
    }
}
