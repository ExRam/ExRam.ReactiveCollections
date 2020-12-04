using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListTransformationReactiveCollection<TSource, TResult> : TransformationReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>, ICanProjectList<TResult>, ICanSortList<TResult>, ICanSortSet<TResult>
    {
        public ListTransformationReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, new ListReactiveCollectionSource<TResult>(), filter, selector, equalityComparer)
        {
        }

        public IReactiveCollection<ListChangedNotification<TChainedResult>> Select<TChainedResult>(Func<TResult, TChainedResult> selector, IEqualityComparer<TChainedResult> equalityComparer)
        {
            return new ListTransformationReactiveCollection<TSource, TChainedResult>(
                Source,
                Filter,
                Selector != null
                    ? (Func<TSource, TChainedResult>)(x => selector(Selector(x)))
                    : x => selector((TResult)(object)x),
                equalityComparer);
        }

        IReactiveCollection<ListChangedNotification<TResult>> ICanSortList<TResult>.Sort(IComparer<TResult> comparer)
        {
            return new SortedListTransformationReactiveCollection<TSource,TResult>(
                Source,
                Filter,
                Selector,
                comparer,
                EqualityComparer);
        }

        IReactiveCollection<SortedSetChangedNotification<TResult>> ICanSortSet<TResult>.Sort(IComparer<TResult> comparer)
        {
            return new SortedSetTransformationReactiveCollection<TSource, TResult>(
                 Source,
                 Filter,
                 Selector,
                 comparer);
        }
    }
}