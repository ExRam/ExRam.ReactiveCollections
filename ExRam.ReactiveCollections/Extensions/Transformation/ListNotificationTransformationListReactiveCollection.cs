using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>, ICanFilter<TSource>, ICanProjectList<TResult>
    {
        public ListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, new ListReactiveCollectionSource<TResult>(), filter, selector, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
        }

        public IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate)
        {
            return this.Selector == null
                ? new ListNotificationTransformationListReactiveCollection<TSource, TResult>(this.Source, x => this.Filter(x) && predicate(x), null, this.EqualityComparer)
                : null;
        }

        public IReactiveCollection<ListChangedNotification<TChainedResult>> TrySelect<TChainedResult>(Func<TResult, TChainedResult> selector, IEqualityComparer<TChainedResult> equalityComparer)
        {
            return new ListNotificationTransformationListReactiveCollection<TSource, TChainedResult>(
                this.Source,
                this.Filter,
                this.Selector != null
                    ? (Func<TSource, TChainedResult>)(x => selector(this.Selector(x)))
                    : x => selector((TResult)(object)x),
                equalityComparer);
        }
    }
}