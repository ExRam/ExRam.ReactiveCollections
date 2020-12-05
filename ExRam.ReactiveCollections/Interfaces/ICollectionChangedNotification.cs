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

    //internal interface ICollectionChangedNotification<out TSelf, T> : ICollectionChangedNotification<T>
    //    where TSelf : ICollectionChangedNotification<TSelf, T>
    //{
    //    TSelf InsertRange(int index, IEnumerable<T> items);

    //    TSelf Sort(int index, int count, IComparer<T> comparer);

    //    TSelf SetItem(int index, T value);

    //    TSelf Reverse(int index, int count);

    //    TSelf RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer);

    //    TSelf RemoveRange(int index, int count);

    //    TSelf RemoveAt(int index);

    //    TSelf RemoveAll(Predicate<T> match);

    //    TSelf Insert(int index, T item);

    //    TSelf Clear();

    //    TSelf Remove(T item, IEqualityComparer<T> equalityComparer);
    //}
}
