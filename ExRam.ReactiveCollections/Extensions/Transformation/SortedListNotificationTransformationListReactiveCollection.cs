using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        private readonly IComparer<TResult> _comparer;
        private readonly IEqualityComparer<TResult> _equalityComparer;

        public SortedListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer, IEqualityComparer<TResult> equalityComparer) : base(source, filter, selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Requires(equalityComparer != null);

            this._comparer = comparer;
            this._equalityComparer = equalityComparer;
        }

        protected override void AddRange(SortedListReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            collection.AddRange(items);
        }

        protected override void Clear(SortedListReactiveCollectionSource<TResult> collection)
        {
            collection.Clear();
        }

        protected override SortedListReactiveCollectionSource<TResult> CreateCollection()
        {
            return new SortedListReactiveCollectionSource<TResult>(this._comparer);
        }

        protected override void InsertRange(SortedListReactiveCollectionSource<TResult> collection, int index, IEnumerable<TResult> items)
        {
            throw new InvalidOperationException();
        }
       
        protected override void RemoveRange(SortedListReactiveCollectionSource<TResult> collection, int index, int count)
        {
            throw new InvalidOperationException();
        }

        protected override void SetItem(SortedListReactiveCollectionSource<TResult> collection, int index, TResult item)
        {
            throw new InvalidOperationException();
        }

        protected override void RemoveRange(SortedListReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            collection.RemoveRange(items, this._equalityComparer);
        }

        protected override void Replace(SortedListReactiveCollectionSource<TResult> collection, TResult oldItem, TResult newItem)
        {
            collection.Replace(oldItem, newItem, this._equalityComparer);
        }

        protected override bool CanHandleIndexes => false;
    }
}