// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return source.Where(filter, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            var nonProjected = source as ListNotificationTransformationReactiveCollection<TSource, TSource>;

            return (nonProjected != null) && (nonProjected.Selector == null)
                ? new ListNotificationTransformationReactiveCollection<TSource, TSource>(nonProjected.Source, x => nonProjected.Filter(x) && filter(x), null, nonProjected.EqualityComparer)
                : new ListNotificationTransformationReactiveCollection<TSource, TSource>(source, filter, null, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<KeyValuePair<TKey, TValue>> filter)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<DictionaryChangedNotification<TKey, TValue>>>() != null);

            return new DictionaryNotificationTransformationReactiveCollection<TKey, TValue, TValue>(source, new DictionaryReactiveCollectionSource<TKey, TValue>(), filter, null, EqualityComparer<KeyValuePair<TKey, TValue>>.Default);
        }
    }
}
