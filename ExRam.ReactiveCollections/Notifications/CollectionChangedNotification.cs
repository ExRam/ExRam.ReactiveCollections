// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ExRam.ReactiveCollections
{
    public abstract class CollectionChangedNotification<T> : ICollectionChangedNotification<T>
    {
        protected CollectionChangedNotification(IReadOnlyCollection<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems)
        {
            Action = action;
            Current = current;
            OldItems = oldItems;
            NewItems = newItems;
        }

        public abstract ICollectionChangedNotification<T> ToResetNotification();

        public IReadOnlyList<T> OldItems { get; }

        public IReadOnlyList<T> NewItems { get; }

        public NotifyCollectionChangedAction Action { get; }

        public IReadOnlyCollection<T> Current { get; }

        ICollectionChangedNotification ICollectionChangedNotification.ToResetNotification() => ToResetNotification();

        IEnumerable ICollectionChangedNotification.Current => Current;

        IEnumerable ICollectionChangedNotification.NewItems => NewItems;

        IEnumerable ICollectionChangedNotification.OldItems => OldItems;
    }
}
