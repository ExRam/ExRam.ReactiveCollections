// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    public sealed class SortedSetChangedNotification<T> : CollectionChangedNotification<T>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
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

        public new ImmutableSortedSet<T> Current => (ImmutableSortedSet<T>)base.Current;
    }
}
