using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedListTransformationReactiveCollection<TSource, TResult> : TransformationReactiveCollection<TSource, TResult, SortedListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        public SortedListTransformationReactiveCollection([NotNull] IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, [NotNull] IComparer<TResult> comparer, [NotNull] IEqualityComparer<TResult> equalityComparer) : base(source, new SortedListReactiveCollectionSource<TResult>(comparer), filter, selector, equalityComparer)
        {
        }
    }
}