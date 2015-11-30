// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<SortedSetChangedNotification<TSource>>>() != null);

            return source.SortSet(Comparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<SortedSetChangedNotification<TSource>>>() != null);

            return new SortedSetNotificationTransformationListReactiveCollection<TSource, TSource>(source, null, null, comparer);
        }
    }
}
