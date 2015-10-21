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
    public sealed class DictionaryChangedNotification<TKey, TValue> : CollectionChangedNotification<KeyValuePair<TKey, TValue>>
    {
        public DictionaryChangedNotification(ImmutableDictionary<TKey, TValue> current, NotifyCollectionChangedAction action, IReadOnlyList<KeyValuePair<TKey, TValue>> oldItems, IReadOnlyList<KeyValuePair<TKey, TValue>> newItems) : base(current, action, oldItems, newItems)
        {
            Contract.Requires(current != null);
            Contract.Requires(oldItems != null);
            Contract.Requires(newItems != null);
        }

        public override ICollectionChangedNotification<KeyValuePair<TKey, TValue>> ToResetNotification()
        {
            return new DictionaryChangedNotification<TKey, TValue>(this.Current, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty);
        }

        public new ImmutableDictionary<TKey, TValue> Current => (ImmutableDictionary<TKey, TValue>)base.Current;
    }
}
