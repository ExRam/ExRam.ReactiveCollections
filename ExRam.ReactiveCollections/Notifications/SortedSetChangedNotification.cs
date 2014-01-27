using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    public sealed class SortedSetChangedNotification<T> : CollectionChangedNotification<ImmutableSortedSet<T>, T>,
        ICollectionChangedNotification<T>
    {
        public SortedSetChangedNotification(ImmutableSortedSet<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems) : base(current, action, oldItems, newItems)
        {
            Contract.Requires(current != null);
            Contract.Requires(oldItems != null);
            Contract.Requires(newItems != null);
        }

        public override ICollectionChangedNotification<T> ToResetNotification()
        {
            return new SortedSetChangedNotification<T>(this.Current, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty);
        }

        IReadOnlyCollection<T> ICollectionChangedNotification<T>.Current
        {
            get
            {
                return this.Current;
            }
        }
    }
}
