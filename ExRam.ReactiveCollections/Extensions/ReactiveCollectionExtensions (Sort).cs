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
        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return source.Sort(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return source.Sort(comparer, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return source.Sort(Comparer<TSource>.Default, equalityComparer);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return new SortedListNotificationTransformationListReactiveCollection<TSource, TSource>(source, null, null, comparer, equalityComparer);
        }
    }
}
