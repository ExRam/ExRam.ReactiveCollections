using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        public SortedListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer, IEqualityComparer<TResult> equalityComparer) : base(source, new SortedListReactiveCollectionSource<TResult>(comparer), filter, selector, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Requires(equalityComparer != null);
        }

        protected override IReactiveCollection<ICollectionChangedNotification> Chain<TNewResult>(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TNewResult> selector, IEqualityComparer<TNewResult> equalityComparer)
        {
            throw new NotSupportedException();
        }

        protected override bool CanAddSelect()
        {
            return false;
        }

        protected override bool CanAddWhere()
        {
            return false;
        }
    }
}