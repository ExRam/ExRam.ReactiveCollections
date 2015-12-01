using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanReplaceValue<T>
    {
        void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer);
    }
}