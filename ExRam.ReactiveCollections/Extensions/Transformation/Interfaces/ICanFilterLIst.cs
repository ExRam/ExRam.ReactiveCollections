using System;

namespace ExRam.ReactiveCollections
{
    internal interface ICanFilterList<TSource>
    {
        IReactiveCollection<ListChangedNotification<TSource>> TryWhere(Predicate<TSource> predicate);
    }
}