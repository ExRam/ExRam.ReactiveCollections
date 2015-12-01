using System;

namespace ExRam.ReactiveCollections
{
    internal interface ICanFilter<out TSource>
    {
        IReactiveCollection<ICollectionChangedNotification> TryWhere(Predicate<TSource> predicate);
    }
}