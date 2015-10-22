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
        #region SortedListReactiveCollection
        private sealed class SortedListReactiveCollection<TSource> : SortedReactiveCollection<SortedListReactiveCollectionSource<TSource>, ListChangedNotification<TSource>, TSource>
        {
            private readonly IEqualityComparer<TSource> _equalityComparer;

            public SortedListReactiveCollection(IObservable<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer) : base(source, comparer)
            {
                Contract.Requires(source != null);
                Contract.Requires(comparer != null);
                Contract.Requires(equalityComparer != null);

                this._equalityComparer = equalityComparer;
            }

            protected override void Add(SortedListReactiveCollectionSource<TSource> collection, TSource item)
            {
                collection.Add(item);
            }

            protected override void AddRange(SortedListReactiveCollectionSource<TSource> collection, IEnumerable<TSource> items)
            {
                collection.AddRange(items);
            }

            protected override void RemoveRange(SortedListReactiveCollectionSource<TSource> collection, IEnumerable<TSource> items)
            {
                collection.RemoveRange(items, this._equalityComparer);
            }

            protected override void Remove(SortedListReactiveCollectionSource<TSource> collection, TSource oldItem)
            {
                collection.Remove(oldItem, this._equalityComparer);
            }

            protected override void Replace(SortedListReactiveCollectionSource<TSource> collection, TSource oldItem, TSource newItem)
            {
                collection.Replace(oldItem, newItem);
            }

            protected override SortedListReactiveCollectionSource<TSource> CreateCollection(IComparer<TSource> comparer)
            {
                return new SortedListReactiveCollectionSource<TSource>(comparer);
            }

            protected override void Clear(SortedListReactiveCollectionSource<TSource> collection)
            {
                collection.Clear();
            }
        }
        #endregion

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

            return new SortedListReactiveCollection<TSource>(source.Changes, comparer, equalityComparer);
        }
    }
}
