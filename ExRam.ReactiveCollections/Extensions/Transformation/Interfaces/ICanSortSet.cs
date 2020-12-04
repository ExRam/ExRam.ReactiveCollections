using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanSortSet<T>
    {
        IReactiveCollection<SortedSetChangedNotification<T>> Sort(IComparer<T> comparer);
    }
}