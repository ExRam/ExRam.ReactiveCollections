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
            // ReSharper disable once SuspiciousTypeConversion.Global
            return source is ICanProjectList<TSource> canProject
                ? canProject.Select(selector, equalityComparer)
                : new ListTransformationReactiveCollection<TSource, TResult>(source, null, selector, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> Select<TKey, TSource, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TSource>> source, Func<TSource, TResult> selector)
            where TKey : notnull
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            return source is ICanProjectDictionary<TKey, TSource> canProject
                ? canProject.Select(selector)
                : new DictionaryTransformationReactiveCollection<TKey, TSource, TResult>(source, null, kvp => new KeyValuePair<TKey, TResult>(kvp.Key, selector(kvp.Value)), EqualityComparer<KeyValuePair<TKey, TResult>>.Default);
        }
    }
}
