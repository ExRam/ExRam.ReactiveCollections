// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region WhereReactiveList
        private sealed class WhereReactiveList<T> : IReactiveCollection<ListChangedNotification<T>>
        {
            private readonly IEqualityComparer<T> _equalityComparer;
            private readonly IObservable<ListChangedNotification<T>> _changes;

            public WhereReactiveList(IObservable<ICollectionChangedNotification<T>> source, Predicate<T> filter, IEqualityComparer<T> equalityComparer)
            {
                Contract.Requires(source != null);
                Contract.Requires(filter != null);
                Contract.Requires(equalityComparer != null);

                this._equalityComparer = equalityComparer;

                this._changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var resultList = new ListReactiveCollectionSource<T>();

                        return Observable
                            .Using(
                                () => source.Subscribe(
                                    notification =>
                                    {
                                        lock (syncRoot)
                                        {
                                            switch (notification.Action)
                                            {
                                                case (NotifyCollectionChangedAction.Add):
                                                {
                                                    resultList.AddRange(notification.NewItems.Where(x => filter(x)));

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Remove):
                                                {
                                                    resultList.RemoveRange(notification.OldItems.Where(x => filter(x)), this._equalityComparer);

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Replace):
                                                {
                                                    if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                    {
                                                        var wasIn = filter(notification.OldItems[0]);
                                                        var getsIn = filter(notification.NewItems[0]);

                                                        if ((wasIn) && (getsIn))
                                                            resultList.Replace(notification.OldItems[0], notification.NewItems[0], this._equalityComparer);
                                                        else if (wasIn)
                                                            resultList.Remove(notification.OldItems[0], this._equalityComparer);
                                                        else if (getsIn)
                                                            resultList.Add(notification.NewItems[0]);
                                                    }
                                                    else
                                                    {
                                                        resultList
                                                            .RemoveRange(notification.OldItems.Where(x => filter(x)), this._equalityComparer);

                                                        resultList
                                                            .AddRange(notification.NewItems.Where(x => filter(x)));
                                                    }

                                                    break;
                                                }

                                                default:
                                                {
                                                    resultList.Clear();
                                                    resultList.AddRange(notification.Current.Where(x => filter(x)));

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

            public IObservable<ListChangedNotification<T>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        #region WhereReactiveDictionary
        private sealed class WhereReactiveDictionary<TKey, TValue> : IReactiveCollection<DictionaryChangedNotification<TKey, TValue>>
        {
            private readonly IObservable<DictionaryChangedNotification<TKey, TValue>> _changes;

            public WhereReactiveDictionary(IObservable<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue> filter)
            {
                this._changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var resultList = new DictionaryReactiveCollectionSource<TKey, TValue>();

                        return Observable
                            .Using(
                                () => source.Subscribe(
                                    notification =>
                                    {
                                        lock (syncRoot)
                                        {
                                            switch (notification.Action)
                                            {
                                                case (NotifyCollectionChangedAction.Add):
                                                {
                                                    resultList.AddRange(notification.NewItems.Where(x => filter(x.Value)));

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Remove):
                                                {
                                                    resultList.RemoveRange(notification.OldItems.Where(x => filter(x.Value)).Select(x => x.Key));

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Replace):
                                                {
                                                    resultList.RemoveRange(notification.OldItems.Where(x => filter(x.Value)).Select(x => x.Key));
                                                    resultList.AddRange(notification.NewItems.Where(x => filter(x.Value)));

                                                    break;
                                                }

                                                default:
                                                {
                                                    resultList.Clear();
                                                    resultList.AddRange(notification.Current.Where(x => filter(x.Value)));

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

            public IObservable<DictionaryChangedNotification<TKey, TValue>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return source.Where(filter, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>> Where<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TSource>>>() != null);

            return new WhereReactiveList<TSource>(source.Changes, filter, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<TValue> filter)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<DictionaryChangedNotification<TKey, TValue>>>() != null);

            return new WhereReactiveDictionary<TKey, TValue>(source.Changes, filter);
        }
    }
}
