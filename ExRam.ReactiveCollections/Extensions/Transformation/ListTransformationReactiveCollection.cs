using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListTransformationReactiveCollection<TSource, TResult> : TransformationReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>, ICanProjectList<TResult>, ICanSortList<TResult>, ICanSortSet<TResult>
    {
        public ListTransformationReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, new ListReactiveCollectionSource<TResult>(), filter, selector, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
        }

        public IReactiveCollection<ListChangedNotification<TChainedResult>> Select<TChainedResult>(Func<TResult, TChainedResult> selector, IEqualityComparer<TChainedResult> equalityComparer)
        {
            return new ListTransformationReactiveCollection<TSource, TChainedResult>(
                this.Source,
                this.Filter,
                this.Selector != null
                    ? (Func<TSource, TChainedResult>)(x => selector(this.Selector(x)))
                    : x => selector((TResult)(object)x),
                equalityComparer);
        }

        IReactiveCollection<ListChangedNotification<TResult>> ICanSortList<TResult>.Sort(IComparer<TResult> comparer)
        {
            return new SortedListTransformationReactiveCollection<TSource,TResult>(
                this.Source,
                this.Filter,
                this.Selector,
                comparer,
                this.EqualityComparer);
        }

        IReactiveCollection<SortedSetChangedNotification<TResult>> ICanSortSet<TResult>.Sort(IComparer<TResult> comparer)
        {
            return new SortedSetTransformationReactiveCollection<TSource, TResult>(
                 this.Source,
                 this.Filter,
                 this.Selector,
                 comparer);
        }
    }
}