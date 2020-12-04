// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IObservable<TValue> GetValueObservable<TKey, TValue>( this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> reactiveCollection, TKey key)
        {
            return reactiveCollection.Changes
                .Where(x => x.Current.ContainsKey(key))
                .Select(x => x.Current[key]);
        }
    }
}
