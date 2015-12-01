using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    internal sealed class ListNotificationTransformationListReactiveCollection<TSource, TResult> : TransformationListReactiveCollection<TSource, TResult, ListReactiveCollectionSource<TResult>, ListChangedNotification<TResult>>, ICanFilter<TSource>, ICanProject<TResult>
    {
        public ListNotificationTransformationListReactiveCollection(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer) : base(source, new ListReactiveCollectionSource<TResult>(), filter, selector, equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
        }

        public IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate)
        {
            return this.CanAddWhere()
                ? this.Chain(this.Source, x => this.Filter(x) && predicate(x), null, this.EqualityComparer)
                : null;
        }

        public IReactiveCollection<ICollectionChangedNotification> TrySelect<TChainedResult>(Func<TResult, TChainedResult> selector, IEqualityComparer<TChainedResult> equalityComparer)
        {
            if (this.CanAddSelect())
            {
                return this.Chain(
                    this.Source,
                    this.Filter,
                    this.Selector != null
                        ? (Func<TSource, TChainedResult>)(x => selector(this.Selector(x)))
                        : x => selector((TResult)(object)x),
                    equalityComparer);
            }

            return null;
        }

        private bool CanAddWhere()
        {
            return this.Selector == null;
        }

        private bool CanAddSelect()
        {
            return true;
        }

        private IReactiveCollection<ICollectionChangedNotification> Chain<TNewResult>(IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TNewResult> selector, IEqualityComparer<TNewResult> equalityComparer)
        {
            return new ListNotificationTransformationListReactiveCollection<TSource, TNewResult>(source, filter, selector, equalityComparer);
        }
    }
}