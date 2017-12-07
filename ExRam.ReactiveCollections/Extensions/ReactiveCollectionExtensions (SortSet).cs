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
        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            return source.SortSet(Comparer<TSource>.Default);
        }

        [NotNull]
        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>([NotNull] this IReactiveCollection<ICollectionChangedNotification<TSource>> source, [NotNull] IComparer<TSource> comparer)
        {
            if (source is ICanSortSet<TSource>)
                return ((ICanSortSet<TSource>)source).Sort(comparer);

            return new SortedSetTransformationReactiveCollection<TSource, TSource>(source, null, null, comparer);
        }
    }
}
