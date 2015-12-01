using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        private readonly IEqualityComparer<TResult> _equalityComparer;

        public SortedListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer, IEqualityComparer<TResult> equalityComparer) : base(source, filter, selector, comparer, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Requires(equalityComparer != null);

            this._equalityComparer = equalityComparer;
        }

        protected override SortedListReactiveCollectionSource<TResult> CreateCollection()
        {
            return new SortedListReactiveCollectionSource<TResult>(this.Comparer);
        }

        public override IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate)
        {
            return this.Selector == null
                ? new SortedListNotificationTransformationListReactiveCollection<TSource, TResult>(this.Source, x => this.Filter(x) && predicate(x), null, this.Comparer, this._equalityComparer)
                : null;
        }
    }
}