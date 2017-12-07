using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedSetTransformationReactiveCollection<TSource, TResult> : TransformationReactiveCollection<TSource, TResult, SortedSetReactiveCollectionSource<TResult>, SortedSetChangedNotification<TResult>>
    {
        public SortedSetTransformationReactiveCollection([NotNull] IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, [NotNull] IComparer<TResult> comparer) : base(source, new SortedSetReactiveCollectionSource<TResult>(comparer), filter, selector, EqualityComparer<TResult>.Default)
        {
        }
    }
}