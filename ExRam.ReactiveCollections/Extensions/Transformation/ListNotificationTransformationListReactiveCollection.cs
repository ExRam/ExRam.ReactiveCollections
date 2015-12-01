using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>, ICanFilterList<TSource>, ICanProjectList<TResult>
    {
        public ListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, new ListReactiveCollectionSource<TResult>(), filter, selector, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
        }

        public IReactiveCollection<ListChangedNotification<TSource>> TryWhere(Predicate<TSource> predicate)
        {
            var nonProjected = this as ListNotificationTransformationListReactiveCollection<TSource, TSource>;

            return ((nonProjected != null) && (nonProjected.Selector == null))
                ? new ListNotificationTransformationListReactiveCollection<TSource, TSource>(nonProjected.Source, x => nonProjected.Filter(x) && predicate(x), null, nonProjected.EqualityComparer)
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