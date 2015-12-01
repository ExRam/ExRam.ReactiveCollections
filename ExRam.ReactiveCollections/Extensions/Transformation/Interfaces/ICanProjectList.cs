using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanProjectList<out TSource>
    {
        IReactiveCollection<ListChangedNotification<TResult>> TrySelect<TResult>(Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer);
    }
}