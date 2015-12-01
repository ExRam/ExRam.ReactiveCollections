using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedSetNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedSetReactiveCollectionSource<TResult>, SortedSetChangedNotification<TResult>>
    {
        public SortedSetNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer) : base(source, new SortedSetReactiveCollectionSource<TResult>(comparer), filter, selector, EqualityComparer<TResult>.Default)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
        }
    }
}