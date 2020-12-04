using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanSortList<T>
    {
        IReactiveCollection<ListChangedNotification<T>> Sort(IComparer<T> comparer);
    }
}