using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanHandleRanges<T>
    {
        void AddRange(IEnumerable<T> items);
        void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer);
    }
}