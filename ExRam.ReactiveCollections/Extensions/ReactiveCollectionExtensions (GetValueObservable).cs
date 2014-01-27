using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IObservable<TValue> GetValueObservable<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>, KeyValuePair<TKey, TValue>> reactiveCollection, TKey key)
        {
            Contract.Requires(reactiveCollection != null);

            return reactiveCollection.Changes
                .Where(x => x.Current.ContainsKey(key))
                .Select(x => x.Current[key]);
        }
    }
}
