using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter)
        {
            return source.WhereSelect(filter, _ => _, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue> filter)
            where TKey : notnull
        {
            return source.WhereSelect(filter, _ => _);
        }

        internal static IReactiveCollection<ListChangedNotification<TResult>> WhereSelect<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource>? filter, Func<TSource, TResult> selector, IEqualityComparer<TResult>? equalityComparer = null)
        {
            return source
                .Transform(
                    ListChangedNotification<TResult>.Reset,
                    (source, target) => target.WhereSelect(source, filter, selector));
        }

        internal static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> WhereSelect<TKey, TValue, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue>? filter, Func<TValue, TResult> selector)
            where TKey : notnull
        {
            return source
                .Transform(
                    DictionaryChangedNotification<TKey, TResult>.Reset,
                    (source, target) => target.WhereSelect(source, filter, selector));
        }
    }
}
