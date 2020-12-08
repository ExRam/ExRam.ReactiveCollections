using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace ExRam.ReactiveCollections.Tests
{
    public class SortedSetReactiveCollectionSourceTest : VerifyBase
    {
        public SortedSetReactiveCollectionSourceTest() : base()
        {

        }
        
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

            var notificationsTask = await list.ReactiveCollection.Changes
                .FirstAsync()
                .ToTask();

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(2)
                .ToArray()
                .ToTask();

            list.Add(1);

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Add_multiple()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(7)
                .Select(x => x.Index)
                .ToArray()
                .ToTask();

            list.Add(1); // 0
            list.Add(3); // 1
            list.Add(2); // 1
            list.Add(0); // 0
            list.Add(6); // 4
            list.Add(5); // 4

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Clear()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(3)
                .FirstAsync()
                .ToTask();

            list.Add(1);
            list.Clear();

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Clear_preserves_comparer()
        {
            var list = new SortedSetReactiveCollectionSource<StructNotImplementingIComparable>(new Comparison<StructNotImplementingIComparable>((x, y) => x.Value.CompareTo(y.Value)).ToComparer());

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(5)
                .ToArray()
                .ToTask();
            
            list.Add(new StructNotImplementingIComparable(1));
            list.Clear();
            list.Add(new StructNotImplementingIComparable(2));
            list.Add(new StructNotImplementingIComparable(1));

            await Verify(notificationsTask);
        }

        [Fact]
        public async Task Remove()
        {
            var list = new SortedSetReactiveCollectionSource<int>();

            var notificationsTask = list.ReactiveCollection.Changes
                .Take(3)
                .ToArray()
                .ToTask();

            list.Add(1);
            list.Remove(1);

            await Verify(notificationsTask);
        }
    }
}
