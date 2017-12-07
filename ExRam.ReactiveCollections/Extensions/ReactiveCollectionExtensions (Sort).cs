// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        [NotNull]
        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            return source.Sort(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        [NotNull]
        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source, [NotNull] IComparer<TSource> comparer)
        {
            return source.Sort(comparer, EqualityComparer<TSource>.Default);
        }

        [NotNull]
        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source, [NotNull] IEqualityComparer<TSource> equalityComparer)
        {
            return source.Sort(Comparer<TSource>.Default, equalityComparer);
        }

        [NotNull]
        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source, [NotNull] IComparer<TSource> comparer, [NotNull] IEqualityComparer<TSource> equalityComparer)
        {
            if (source is ICanSortList<TSource>)
                return ((ICanSortList<TSource>)source).Sort(comparer);

            return new SortedListTransformationReactiveCollection<TSource, TSource>(source, null, null, comparer, equalityComparer);
        }
    }
}
