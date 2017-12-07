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
    public interface ICollectionChangedNotification
    {
        [NotNull]
        ICollectionChangedNotification ToResetNotification();

        [NotNull]
        IEnumerable OldItems
        {
            get;
        }

        [NotNull]
        IEnumerable NewItems
        {
            get;
        }

        NotifyCollectionChangedAction Action
        {
            get;
        }

        [NotNull]
        IEnumerable Current
        {
            get;
        }
    }

    public interface ICollectionChangedNotification<out T> : ICollectionChangedNotification
    {
        [NotNull]
        new ICollectionChangedNotification<T> ToResetNotification();

        [NotNull]
        new IReadOnlyList<T> OldItems
        {
            get;
        }

        [NotNull]
        new IReadOnlyList<T> NewItems
        {
            get;
        }

        [NotNull]
        new IReadOnlyCollection<T> Current
        {
            get;
        }
    }
}
