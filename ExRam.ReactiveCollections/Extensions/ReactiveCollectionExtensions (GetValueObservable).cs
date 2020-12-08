using System;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IObservable<TValue> GetValues<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> reactiveCollection, TKey key)
            where TKey : notnull
        {
            return reactiveCollection.Changes
                .Where(x => x.Current.ContainsKey(key))
                .Select(x => x.Current[key]);
        }
    }
}
