// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter)
        {
            return source.Where(filter, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, IEqualityComparer<TSource> equalityComparer)
        {
            return source is ListTransformationReactiveCollection<TSource, TSource> { Selector: null, Filter: not null } nonProjected
                ? new ListTransformationReactiveCollection<TSource, TSource>(nonProjected.Source, x => nonProjected.Filter!(x) && filter(x), null, nonProjected.EqualityComparer)
                : new ListTransformationReactiveCollection<TSource, TSource>(source, filter, null, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue> filter)
            where TKey : notnull
        {
            return source is DictionaryTransformationReactiveCollection<TKey, TValue, TValue> { Selector: null, Filter: not null } nonProjected
                ? new DictionaryTransformationReactiveCollection<TKey, TValue, TValue>(nonProjected.Source, kvp => nonProjected.Filter!(kvp) && filter(kvp.Value), null, nonProjected.EqualityComparer)
                : new DictionaryTransformationReactiveCollection<TKey, TValue, TValue>(source, kvp => filter(kvp.Value), null, EqualityComparer<KeyValuePair<TKey, TValue>>.Default);
        }
    }
}
