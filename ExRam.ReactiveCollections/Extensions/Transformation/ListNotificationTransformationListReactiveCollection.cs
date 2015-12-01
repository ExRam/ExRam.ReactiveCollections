using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>, ICanProject<TResult>
    {
        private readonly IEqualityComparer<TResult> _equalityComparer;

        public ListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, filter, selector, null, equalityComparer)
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

        public IReactiveCollection<ICollectionChangedNotification> TrySelect<TChainedResult>(Func<TResult, TChainedResult> selector, IEqualityComparer<TChainedResult> equalityComparer)
        {
            return new ListNotificationTransformationListReactiveCollection<TSource, TChainedResult>(
                this.Source,
                this.Filter,
                this.Selector != null
                    ? (Func<TSource, TChainedResult>)(x => selector(this.Selector(x)))
                    : x => selector((TResult)(object)x),
                equalityComparer);
        }

        protected override ListReactiveCollectionSource<TResult> CreateCollection()
        {
            return new ListReactiveCollectionSource<TResult>();
        }
    }
}