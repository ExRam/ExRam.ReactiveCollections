using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedListTransformationReactiveCollection<TSource, TResult> : TransformationReactiveCollection<TSource, TResult, SortedListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        public SortedListTransformationReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer, IEqualityComparer<TResult> equalityComparer) : base(source, new SortedListReactiveCollectionSource<TResult>(comparer), filter, selector, equalityComparer)
        {
        }
    }
}