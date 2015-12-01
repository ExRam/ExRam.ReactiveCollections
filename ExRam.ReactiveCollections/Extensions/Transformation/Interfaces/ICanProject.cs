using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanProject<out TSource>
    {
        IReactiveCollection<ICollectionChangedNotification> TrySelect<TResult>(Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer);
    }
}