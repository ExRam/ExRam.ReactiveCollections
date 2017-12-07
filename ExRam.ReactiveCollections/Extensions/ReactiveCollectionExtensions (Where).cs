// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        [NotNull]
        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source, [NotNull] Predicate<TSource> filter)
        {
            return source.Where(filter, EqualityComparer<TSource>.Default);
        }

        [NotNull]
        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source, [NotNull] Predicate<TSource> filter, [NotNull] IEqualityComparer<TSource> equalityComparer)
        {
            return source is ListTransformationReactiveCollection<TSource, TSource> nonProjected && nonProjected.Selector == null
                ? new ListTransformationReactiveCollection<TSource, TSource>(nonProjected.Source, x => nonProjected.Filter(x) && filter(x), null, nonProjected.EqualityComparer)
                : new ListTransformationReactiveCollection<TSource, TSource>(source, filter, null, equalityComparer);
        }

        [NotNull]
        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>([NotNull] this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, [NotNull] Predicate<TValue> filter)
        {
            return source is DictionaryTransformationReactiveCollection<TKey, TValue, TValue> nonProjected && nonProjected.Selector == null
                ? new DictionaryTransformationReactiveCollection<TKey, TValue, TValue>(nonProjected.Source, kvp => nonProjected.Filter(kvp) && filter(kvp.Value), null, nonProjected.EqualityComparer)
                : new DictionaryTransformationReactiveCollection<TKey, TValue, TValue>(source, kvp => filter(kvp.Value), null, EqualityComparer<KeyValuePair<TKey, TValue>>.Default);
        }
    }
}
