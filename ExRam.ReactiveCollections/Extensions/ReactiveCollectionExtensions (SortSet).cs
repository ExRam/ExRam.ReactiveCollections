// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region SortReactiveSortedList
        private sealed class SortedSetReactiveCollection<TSource> : IReactiveCollection<SortedSetChangedNotification<TSource>>
        {
            private readonly IObservable<SortedSetChangedNotification<TSource>> _changes;

            public SortedSetReactiveCollection(IObservable<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
            {
                Contract.Requires(source != null);
                Contract.Requires(comparer != null);
                Contract.Requires(equalityComparer != null);

                this._changes = Observable
                    .Defer(
                        () =>
                        {
                            var syncRoot = new object();
                            var resultList = new SortedSetReactiveCollectionSource<TSource>(comparer);

                            return Observable.Using(
                                () => source.Subscribe(
                                    notification =>
                                    {
                                        lock (syncRoot)
                                        {
                                            switch (notification.Action)
                                            {
                                                case (NotifyCollectionChangedAction.Add):
                                                {
                                                    if (notification.NewItems.Count == 1)
                                                        resultList.Add(notification.NewItems[0]);
                                                    else
                                                        resultList.AddRange(notification.NewItems);

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Remove):
                                                {
                                                    foreach (var value in notification.OldItems)
                                                    {
                                                        resultList.Remove(value);
                                                    }

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Replace):
                                                {
                                                    foreach (var value in notification.OldItems)
                                                    {
                                                        resultList.Remove(value);
                                                    }

                                                    foreach (var value in notification.NewItems)
                                                    {
                                                        resultList.Add(value);
                                                    }

                                                    break;
                                                }

                                                default:
                                                {
                                                    resultList.Clear();
                                                    resultList.AddRange(notification.Current);

                                                    break;
                                                }
                                            }
                                        }
                                    }),

                                    _ => resultList.ReactiveCollection.Changes);
                        })
                    .Replay(1)
                    .RefCount()
                    .Normalize();
            }

            public IObservable<SortedSetChangedNotification<TSource>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<SortedSetChangedNotification<TSource>>>() != null);

            return source.SortSet(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<SortedSetChangedNotification<TSource>>>() != null);

            return source.SortSet(comparer, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<SortedSetChangedNotification<TSource>>>() != null);

            return source.SortSet(Comparer<TSource>.Default, equalityComparer);
        }

        public static IReactiveCollection<SortedSetChangedNotification<TSource>> SortSet<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<SortedSetChangedNotification<TSource>>>() != null);

            return new SortedSetReactiveCollection<TSource>(source.Changes, comparer, equalityComparer);
        }
    }
}
