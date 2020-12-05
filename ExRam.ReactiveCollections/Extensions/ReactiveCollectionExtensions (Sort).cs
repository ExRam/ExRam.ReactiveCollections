// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            return source.Sort(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            return source.Sort(comparer, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IEqualityComparer<TSource> equalityComparer)
        {
            return source.Sort(Comparer<TSource>.Default, equalityComparer);
        }

        public static IReactiveCollection<SortedListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
        {
            return source
                .Changes
                .Scan(
                    new[] { SortedListChangedNotification<TSource>.Reset.WithComparer(comparer) },
                    (currentTargetNotification, sourceNotification) =>
                    {
                        var newRet = currentTargetNotification[^1]
                            .Sort(sourceNotification)
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
