using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        private readonly IEqualityComparer<TResult> _equalityComparer;

        public ListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, filter, selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);

            this._equalityComparer = equalityComparer;
        }

        public override IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate)
        {
            return this.Selector == null
                ? new ListNotificationTransformationListReactiveCollection<TSource, TResult>(this.Source, x => this.Filter(x) && predicate(x), null, this._equalityComparer)
                : null;
        }

        public ListNotificationTransformationListReactiveCollection<TSource, TChainedResult> AddSelect<TChainedResult>(Func<TResult, TChainedResult> selector, IEqualityComparer<TChainedResult> equalityComparer)
        {
            return new ListNotificationTransformationListReactiveCollection<TSource, TChainedResult>(
                this.Source,
                this.Filter,
                this.Selector != null 
                    ? (Func<TSource, TChainedResult>)(x => selector(this.Selector(x)))
                    : x => selector((TResult)(object)x),
                equalityComparer);
        }

        protected override void SetItem(ListReactiveCollectionSource<TResult> collection, int index, TResult item)
        {
            collection.SetItem(index, item);
        }

        protected override void AddRange(ListReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            collection.AddRange(items);
        }

        protected override void Clear(ListReactiveCollectionSource<TResult> collection)
        {
            collection.Clear();
        }

        protected override void InsertRange(ListReactiveCollectionSource<TResult> collection, int index, IEnumerable<TResult> items)
        {
            collection.InsertRange(index, items);
        }

        protected override void RemoveRange(ListReactiveCollectionSource<TResult> collection, int index, int count)
        {
            collection.RemoveRange(index, count);
        }

        protected override void RemoveRange(ListReactiveCollectionSource<TResult> collection, IEnumerable<TResult> items)
        {
            collection.RemoveRange(items, this._equalityComparer);
        }

        protected override void Replace(ListReactiveCollectionSource<TResult> collection, TResult oldItem, TResult newItem)
        {
            collection.Replace(oldItem, newItem, this._equalityComparer);
        }

        protected override ListReactiveCollectionSource<TResult> CreateCollection()
        {
            return new ListReactiveCollectionSource<TResult>();
        }

        protected override bool CanHandleIndexes => true;
    }
}