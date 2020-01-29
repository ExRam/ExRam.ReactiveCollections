// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public sealed class ListChangedNotification<T> : CollectionChangedNotification<T>, IIndexedCollectionChangedNotification<T>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public ListChangedNotification([NotNull] ImmutableList<T> current, NotifyCollectionChangedAction action, [NotNull] IReadOnlyList<T> oldItems, [NotNull] IReadOnlyList<T> newItems, int? index) : base(current, action, oldItems, newItems)
        {
            Index = index;
        }

        public override ICollectionChangedNotification<T> ToResetNotification()
        {
            return new ListChangedNotification<T>(Current, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public int? Index { get; }

        public new ImmutableList<T> Current => (ImmutableList<T>)base.Current;
    }
}
