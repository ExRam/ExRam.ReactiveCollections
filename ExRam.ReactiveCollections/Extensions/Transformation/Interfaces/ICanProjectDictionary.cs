using System;

namespace ExRam.ReactiveCollections
{
    internal interface ICanProjectDictionary<TKey, out TSource>
    {
        IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> Select<TResult>(Func<TSource, TResult> selector);
    }
}