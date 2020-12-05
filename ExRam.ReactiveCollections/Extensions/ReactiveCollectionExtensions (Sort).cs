// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            return source.Sort(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            return source.Sort(comparer, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IEqualityComparer<TSource> equalityComparer)
        {
            return source.Sort(Comparer<TSource>.Default, equalityComparer);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
        {
            return source is ICanSortList<TSource> 
                ? ((ICanSortList<TSource>)source).Sort(comparer)
                : new SortedListTransformationReactiveCollection<TSource, TSource>(source, null, null, comparer, equalityComparer);
        }
    }
}
