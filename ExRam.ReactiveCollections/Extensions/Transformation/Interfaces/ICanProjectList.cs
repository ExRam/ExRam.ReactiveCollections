using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal interface ICanProjectList<out TSource>
    {
        IReactiveCollection<ListChangedNotification<TResult>> Select<TResult>(Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer);
    }
}