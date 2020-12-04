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
        public static IReactiveCollection<ListChangedNotification<TResult>> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Func<TSource, TResult> selector)
        {
            return source.Select(selector, EqualityComparer<TResult>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TResult>> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer)
        {
            var ret = (source as ICanProjectList<TSource>)?.Select(selector, equalityComparer);

            return ret ?? new ListTransformationReactiveCollection<TSource, TResult>(source, null, selector, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> Select<TKey, TSource, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TSource>> source, Func<TSource, TResult> selector)
        {
            var ret = (source as ICanProjectDictionary<TKey, TSource>)?.Select(selector);

            return ret ?? new DictionaryTransformationReactiveCollection<TKey, TSource, TResult>(source, null, kvp => new KeyValuePair<TKey, TResult>(kvp.Key, selector(kvp.Value)), EqualityComparer<KeyValuePair<TKey, TResult>>.Default);
        }
    }
}
