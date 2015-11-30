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
        #region SortedReactiveCollection
        private abstract class SortedReactiveCollection<TCollection, TNotification, TSource> : IReactiveCollection<TNotification>
            where TCollection : IReactiveCollectionSource<TNotification>, new()
            where TNotification : ICollectionChangedNotification
        {
            protected SortedReactiveCollection(IObservable<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer)
            {
                Contract.Requires(source != null);

                this.Changes = Observable
                    .Defer(
                        () =>
                        {
                            var syncRoot = new object();
                            var resultList = this.CreateCollection(comparer);

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
                                                        this.Add(resultList, notification.NewItems[0]);
                                                    else
                                                        this.AddRange(resultList, notification.NewItems);

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Remove):
                                                {
                                                    if (notification.NewItems.Count == 1)
                                                        this.Remove(resultList, notification.NewItems[0]);
                                                    else
                                                        this.RemoveRange(resultList, notification.OldItems);

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Replace):
                                                {
                                                    if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                        this.Replace(resultList, notification.OldItems[0], notification.NewItems[0]);
                                                    else
                                                    {
                                                        this.RemoveRange(resultList, notification.OldItems);
                                                        this.AddRange(resultList, notification.NewItems);
                                                    }

                                                    break;
                                                }

                                                default:
                                                {
                                                    this.Clear(resultList);
                                                    this.AddRange(resultList, notification.Current);

                                                    break;
                                                }
                                            }
                                        }
                                    }),

                                    _ => resultList.ReactiveCollection.Changes);
                        })
                    .ReplayFresh(1)
                    .RefCount()
                    .Normalize();
            }

            protected abstract TCollection CreateCollection(IComparer<TSource> comparer);
            protected abstract void Add(TCollection collection, TSource item);
            protected abstract void AddRange(TCollection collection, IEnumerable<TSource> items);
            protected abstract void RemoveRange(TCollection collection, IEnumerable<TSource> items);
            protected abstract void Remove(TCollection collection, TSource oldItem);
            protected abstract void Replace(TCollection collection, TSource oldItem, TSource newItem);
            protected abstract void Clear(TCollection collection);

            public IObservable<TNotification> Changes { get; }
        }
        #endregion

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
