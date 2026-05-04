using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using static VerifyXunit.Verifier;
using Xunit;

using Shouldly;

namespace ExRam.ReactiveCollections.Tests
{
    public class SortedListReactiveCollectionSourceTest
    {
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

            Assert.True(list.Contains(1));
            Assert.False(list.Contains(2));
        }

        [Fact]
        public void CopyTo()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            var target = new int[5];
            list.CopyTo(target, 2);

            Assert.Equal(new[] { 0, 0, 1, 2, 3 }, target);
        }

        [Fact]
        public void GetEnumerator()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            var range = new[] { 1, 2, 3 };
            list.AddRange(range);

            using (var enumerator = list.GetEnumerator())
            {
                enumerator.MoveNext().ShouldBeTrue();
                enumerator.Current.ShouldBe(1);
                enumerator.MoveNext().ShouldBeTrue();
                enumerator.Current.ShouldBe(2);
                enumerator.MoveNext().ShouldBeTrue();
                enumerator.Current.ShouldBe(3);
                enumerator.MoveNext().ShouldBeFalse();
            }
        }

        [Fact]
        public void IndexOf()
        {
            var list = new SortedListReactiveCollectionSource<int>();

            list.AddRange(new[] { 1, 2, 3 });

            list.IndexOf(3).ShouldBe(2);
            list.IndexOf(2).ShouldBe(1);
            list.IndexOf(1).ShouldBe(0);
        }

        [Fact]
        public void Item()
        {
            var list = new SortedListReactiveCollectionSource<int>();
            list.Add(1);

            list[0].ShouldBe(1);
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
