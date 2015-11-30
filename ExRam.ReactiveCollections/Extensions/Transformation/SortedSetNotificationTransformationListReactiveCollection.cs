using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedSetNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedSetReactiveCollectionSource<TResult>, SortedSetChangedNotification<TResult>>
    {
        public SortedSetNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer) : base(source, filter, selector, comparer, EqualityComparer<TResult>.Default)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
        }

        protected override void AddRange(SortedSetReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            collection.AddRange(items);
        }

        protected override SortedSetReactiveCollectionSource<TResult> CreateCollection()
        {
            return new SortedSetReactiveCollectionSource<TResult>(this.Comparer);
        }

        protected override void InsertRange(SortedSetReactiveCollectionSource<TResult> collection, int index, IEnumerable<TResult> items)
        {
            throw new InvalidOperationException();
        }

        protected override void RemoveRange(SortedSetReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }

        protected override void RemoveRange(SortedSetReactiveCollectionSource<TResult> collection, int index, int count)
        {
            throw new InvalidOperationException();
        }

        public override IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate)
        {
            return this.Selector == null
                ? new SortedSetNotificationTransformationListReactiveCollection<TSource, TResult>(this.Source, x => this.Filter(x) && predicate(x), null, this.Comparer)
                : null;
        }
    }
}