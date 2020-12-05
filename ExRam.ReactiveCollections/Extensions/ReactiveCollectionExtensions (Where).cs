// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

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
            return source.WhereSelect(filter, _ => _, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue> filter)
            where TKey : notnull
        {
            return source.WhereSelect(filter, _ => _);
        }

        internal static IReactiveCollection<ListChangedNotification<TResult>> WhereSelect<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource>? filter, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer)
        {
            return source
                .Changes
                .Scan(
                    new[] { ListChangedNotification<TResult>.Reset },
                    (currentTargetNotification, sourceNotification) =>
                    {
                        var newRet = currentTargetNotification[^1]
                            .WhereSelect(sourceNotification, filter, selector)
                            .ToArray();

                        return newRet.Length > 0 ? newRet :
                            currentTargetNotification.Length > 0
                                ? new[] {currentTargetNotification[0]} 
                                : currentTargetNotification;
                    })
                .SelectMany(x => x)
                .DistinctUntilChanged()
                .ToReactiveCollection();
        }

        internal static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> WhereSelect<TKey, TValue, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue>? filter, Func<TValue, TResult> selector)
            where TKey : notnull
        {
            return source
                .Changes
                .Scan(
                    new[] { DictionaryChangedNotification<TKey, TResult>.Reset },
                    (currentTargetNotification, sourceNotification) =>
                    {
                        var newRet = currentTargetNotification[^1]
                            .WhereSelect(sourceNotification, filter, selector)
                            .ToArray();

                        return newRet.Length > 0 ? newRet :
                            currentTargetNotification.Length > 0
                                ? new[] { currentTargetNotification[0] }
                                : currentTargetNotification;
                    })
                .SelectMany(x => x)
                .DistinctUntilChanged()
                .ToReactiveCollection();
        }
    }
}
