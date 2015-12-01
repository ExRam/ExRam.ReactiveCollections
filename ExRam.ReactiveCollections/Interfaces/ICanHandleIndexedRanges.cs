using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanHandleIndexedRanges<in T>
    {
        void InsertRange(int index, IEnumerable<T> items);
        void RemoveRange(int index, int count);
    }
}