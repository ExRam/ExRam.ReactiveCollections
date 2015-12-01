using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedSetNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedSetReactiveCollectionSource<TResult>, SortedSetChangedNotification<TResult>>
    {
        public SortedSetNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer) : base(source, new SortedSetReactiveCollectionSource<TResult>(comparer), filter, selector, comparer, EqualityComparer<TResult>.Default)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
        }

        protected override IReactiveCollection<ICollectionChangedNotification> Chain<TNewResult>(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TNewResult> selector, IEqualityComparer<TNewResult> equalityComparer)
        {
            throw new NotSupportedException();
        }
    }
}