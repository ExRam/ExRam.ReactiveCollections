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
using System.Reactive;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        #region SortReactiveSortedList
        private sealed class SortedReactiveCollection<TSource> : IReactiveCollection<ListChangedNotification<TSource>, TSource>
        {
            private readonly IObservable<ListChangedNotification<TSource>> _changes;

            public SortedReactiveCollection(IObservable<ICollectionChangedNotification<TSource>> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
            {
                Contract.Requires(source != null);
                Contract.Requires(comparer != null);
                Contract.Requires(equalityComparer != null);

                this._changes = Observable
                    .Create<SortedListReactiveCollectionSource<TSource>>(observer =>
                    {
                        var syncRoot = new object();
                        var resultList = new SortedListReactiveCollectionSource<TSource>(comparer);

                        observer.OnNext(resultList);

                        return source.Subscribe(
                            (notification) =>
                            {
                                lock (syncRoot)
                                {
                                    switch (notification.Action)
                                    {
                                        case (NotifyCollectionChangedAction.Add):
                                        {
                                            resultList.AddRange(notification.NewItems);

                                            break;
                                        }

                                        case (NotifyCollectionChangedAction.Remove):
                                        {
                                            resultList.RemoveRange(notification.OldItems, equalityComparer);

                                            break;
                                        }

                                        case (NotifyCollectionChangedAction.Replace):
                                        {
                                            if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                resultList.Replace(notification.OldItems[0], notification.NewItems[0], equalityComparer);
                                            else
                                            {
                                                foreach (var value in notification.OldItems)
                                                {
                                                    resultList.Remove(value, equalityComparer);
                                                }

                                                foreach (var value in notification.NewItems)
                                                {
                                                    resultList.Add(value);
                                                }
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
                            });
                    })
                    .SelectMany(x => x.ReactiveCollection.Changes)
                    .Replay(1)
                    .RefCount()
                    .Normalize<ListChangedNotification<TSource>, TSource>();
            }

            public IObservable<ListChangedNotification<TSource>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        #region SortedFromDictionaryReactiveSortedList
        private sealed class SortedFromDictionaryReactiveSortedList<TKey, TValue> : IReactiveCollection<ListChangedNotification<TValue>, TValue>
        {
            #region KeyValuePairEqualityComparer
            private sealed class KeyValuePairEqualityComparer : IEqualityComparer<KeyValuePair<TKey, TValue>>
            {
                public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
                {
                    return x.Key.Equals(y.Key);
                }

                public int GetHashCode(KeyValuePair<TKey, TValue> obj)
                {
                    return obj.Key.GetHashCode();
                }
            }
            #endregion

            private static readonly KeyValuePairEqualityComparer EqualityComparer = new KeyValuePairEqualityComparer();

            private readonly object _syncRoot = new object();
            private readonly IObservable<ListChangedNotification<TValue>> _changes;
            private readonly SortedListReactiveCollectionSource<KeyValuePair<TKey, TValue>> _resultList;
 
            public SortedFromDictionaryReactiveSortedList(IObservable<DictionaryChangedNotification<TKey, TValue>> source, IComparer<TValue> comparer)
            {
                Contract.Requires(source != null);
                Contract.Requires(comparer != null);

                this._resultList = new SortedListReactiveCollectionSource<KeyValuePair<TKey, TValue>>(new Comparison<KeyValuePair<TKey, TValue>>((x, y) => comparer.Compare(x.Value, y.Value)).ToComparer());
                
                this._changes = Observable
                    .Using(() => source.Subscribe(
                        (notification) =>
                        {
                            lock (_syncRoot)
                            {
                                switch (notification.Action)
                                {
                                    case (NotifyCollectionChangedAction.Add):
                                    {
                                        this._resultList.AddRange(notification.NewItems);

                                        break;
                                    }

                                    case (NotifyCollectionChangedAction.Remove):
                                    {
                                        this._resultList.RemoveRange(notification.OldItems, EqualityComparer);

                                        break;
                                    }

                                    case (NotifyCollectionChangedAction.Replace):
                                    {
                                        if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                            this._resultList.Replace(notification.OldItems[0], notification.NewItems[0], EqualityComparer);
                                        else
                                        {
                                            // TODO: Performance
                                            this._resultList.RemoveRange(notification.OldItems, EqualityComparer);
                                            this._resultList.AddRange(notification.NewItems);
                                        }

                                        break;
                                    }

                                    default:
                                    {
                                        this._resultList.Clear();
                                        this._resultList.AddRange(notification.Current);
                                            
                                        break;
                                    }
                                }
                            }
                        }),
                        _ => Observable.Return(Unit.Default).Concat(Observable.Never<Unit>()))
                    .SelectMany(_ => this._resultList.ReactiveCollection.Select(y => y.Value).Changes)
                    .Replay(1)
                    .RefCount()
                    .Normalize<ListChangedNotification<TValue>, TValue>();
            }

            public IObservable<ListChangedNotification<TValue>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<ListChangedNotification<TSource>, TSource> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>, TSource> source)
        {
            Contract.Requires(source != null);

            return source.Sort(Comparer<TSource>.Default, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>, TSource> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>, TSource> source, IComparer<TSource> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);

            return source.Sort(comparer, EqualityComparer<TSource>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>, TSource> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>, TSource> source, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);

            return source.Sort(Comparer<TSource>.Default, equalityComparer);
        }

        public static IReactiveCollection<ListChangedNotification<TSource>, TSource> Sort<TSource>(this IReactiveCollection<ICollectionChangedNotification<TSource>, TSource> source, IComparer<TSource> comparer, IEqualityComparer<TSource> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);
            Contract.Requires(equalityComparer != null);

            return new SortedReactiveCollection<TSource>(source.Changes, comparer, equalityComparer);
        }

        public static IReactiveCollection<ListChangedNotification<TValue>, TValue> Sort<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>, KeyValuePair<TKey, TValue>> source)
        {
            Contract.Requires(source != null);

            return new SortedFromDictionaryReactiveSortedList<TKey, TValue>(source.Changes, Comparer<TValue>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TValue>, TValue> Sort<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>, KeyValuePair<TKey, TValue>> source, IComparer<TValue> comparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(comparer != null);

            return new SortedFromDictionaryReactiveSortedList<TKey, TValue>(source.Changes, comparer);
        }
    }
}
