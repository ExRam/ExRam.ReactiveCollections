using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedSetNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedSetReactiveCollectionSource<TResult>, SortedSetChangedNotification<TResult>>
    {
        private readonly IComparer<TResult> _comparer;

        public SortedSetNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer) : base(source, filter, selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);

            this._comparer = comparer;
        }

        protected override void AddRange(SortedSetReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            collection.AddRange(items);
        }

        protected override void Clear(SortedSetReactiveCollectionSource<TResult> collection)
        {
            collection.Clear();
        }

        protected override SortedSetReactiveCollectionSource<TResult> CreateCollection()
        {
            return new SortedSetReactiveCollectionSource<TResult>(this._comparer);
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

        protected override void Replace(SortedSetReactiveCollectionSource<TResult> collection, TResult oldItem, TResult newItem)
        {
            collection.Remove(oldItem);
            collection.Add(newItem);
        }

        protected override void SetItem(SortedSetReactiveCollectionSource<TResult> collection, int index, TResult item)
        {
            throw new InvalidOperationException();
        }

        protected override bool CanHandleIndexes => false;
    }
}