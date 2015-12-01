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
        public static IReactiveCollection<ListChangedNotification<TResult>> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TResult>>>() != null);

            return source.Select(selector, EqualityComparer<TResult>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TResult>> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TResult>>>() != null);

            var ret = (source as ICanProjectList<TSource>)?.Select(selector, equalityComparer);

            return ret ?? new ListNotificationTransformationReactiveCollection<TSource, TResult>(source, null, selector, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> Select<TKey, TSource, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TSource>> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<DictionaryChangedNotification<TKey, TResult>>>() != null);

            return new DictionaryNotificationTransformationReactiveCollection<TKey, TSource, TResult>(source, null, kvp => new KeyValuePair<TKey, TResult>(kvp.Key, selector(kvp.Value)), EqualityComparer<KeyValuePair<TKey, TResult>>.Default);
        }
    }
}
