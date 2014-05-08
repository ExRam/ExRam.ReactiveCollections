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
    public sealed class ListChangedNotification<T> : CollectionChangedNotification<ImmutableList<T>, T>,
        ICollectionChangedNotification<T>
    {
        private readonly int? _index;

        public ListChangedNotification(ImmutableList<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems, int? index) : base(current, action, oldItems, newItems)
        {
            Contract.Requires(current != null);
            Contract.Requires(oldItems != null);
            Contract.Requires(newItems != null);

            this._index = index;
        }

        public override ICollectionChangedNotification<T> ToResetNotification()
        {
            return new ListChangedNotification<T>(this.Current, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public int? Index
        {
            get
            {
                return this._index;
            }
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
