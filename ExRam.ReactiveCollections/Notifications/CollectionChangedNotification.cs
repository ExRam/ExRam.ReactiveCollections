// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public abstract class CollectionChangedNotification<T> : ICollectionChangedNotification<T>
    {
        protected CollectionChangedNotification([NotNull] IReadOnlyCollection<T> current, NotifyCollectionChangedAction action, [NotNull] IReadOnlyList<T> oldItems, [NotNull] IReadOnlyList<T> newItems)
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

        ICollectionChangedNotification ICollectionChangedNotification.ToResetNotification()
        {
            return ToResetNotification();
        }

        IEnumerable ICollectionChangedNotification.Current => Current;

        IEnumerable ICollectionChangedNotification.NewItems => NewItems;

        IEnumerable ICollectionChangedNotification.OldItems => OldItems;
    }
}
