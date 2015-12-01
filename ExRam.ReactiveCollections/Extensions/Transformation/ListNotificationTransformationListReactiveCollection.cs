using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>
    {
        public ListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, new ListReactiveCollectionSource<TResult>(), filter, selector, null, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
        }

        protected override IReactiveCollection<ICollectionChangedNotification> Chain<TNewResult>(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TNewResult> selector, IEqualityComparer<TNewResult> equalityComparer)
        {
            return new ListNotificationTransformationListReactiveCollection<TSource, TNewResult>(source, filter, selector, equalityComparer);
        }
    }
}