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
    public interface ICollectionChangedNotification
    {
        ICollectionChangedNotification ToResetNotification();

        IEnumerable OldItems
        {
            get;
        }

        IEnumerable NewItems
        {
            get;
        }

        NotifyCollectionChangedAction Action
        {
            get;
        }

        IEnumerable Current
        {
            get;
        }
    }

    public interface ICollectionChangedNotification<out T> : ICollectionChangedNotification
    {
        new ICollectionChangedNotification<T> ToResetNotification();

        new IReadOnlyList<T> OldItems
        {
            get;
        }

        new IReadOnlyList<T> NewItems
        {
            get;
        }

        new IReadOnlyCollection<T> Current
        {
            get;
        }
    }
}
