using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class SortedSetNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, SortedSetReactiveCollectionSource<TResult>, SortedSetChangedNotification<TResult>>
    {
        public SortedSetNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IComparer<TResult> comparer) : base(source, filter, selector, comparer, EqualityComparer<TResult>.Default)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
        }

        protected override SortedSetReactiveCollectionSource<TResult> CreateCollection()
        {
            return new SortedSetReactiveCollectionSource<TResult>(this.Comparer);
        }

        public override IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate)
        {
            return this.Selector == null
                ? new SortedSetNotificationTransformationListReactiveCollection<TSource, TResult>(this.Source, x => this.Filter(x) && predicate(x), null, this.Comparer)
                : null;
        }
    }
}