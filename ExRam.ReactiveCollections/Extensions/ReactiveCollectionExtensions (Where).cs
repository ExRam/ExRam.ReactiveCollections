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
        #region WhereReactiveCollection
        private abstract class WhereReactiveCollection<TCollection, TNotification, T> : IReactiveCollection<TNotification>
            where TCollection : IReactiveCollectionSource<TNotification>, new()
            where TNotification : ICollectionChangedNotification
        {
            protected WhereReactiveCollection(IObservable<ICollectionChangedNotification<T>> source, Predicate<T> filter)
            {
                Contract.Requires(source != null);
                Contract.Requires(filter != null);

                this.Changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var resultList = new TCollection();

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
                                                    if (notification.NewItems.Count == 1)
                                                    {
                                                        var newItem = notification.NewItems[0];
                                                        if (filter(newItem))
                                                            this.Add(resultList, newItem);
                                                    }
                                                    else
                                                        this.AddRange(resultList, notification.NewItems.Where(x => filter(x)));

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Remove):
                                                {
                                                    if (notification.OldItems.Count == 1)
                                                    {
                                                        var oldItem = notification.OldItems[0];
                                                        if (filter(oldItem))
                                                            this.Remove(resultList, oldItem);
                                                    }
                                                    else
                                                        this.RemoveRange(resultList, notification.OldItems.Where(x => filter(x)));

                                                    break;
                                                }

                                                case (NotifyCollectionChangedAction.Replace):
                                                {
                                                    if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                    {
                                                        var wasIn = filter(notification.OldItems[0]);
                                                        var getsIn = filter(notification.NewItems[0]);

                                                        if ((wasIn) && (getsIn))
                                                            this.Replace(resultList, notification.OldItems[0], notification.NewItems[0]);
                                                        else if (wasIn)
                                                            this.Remove(resultList, notification.OldItems[0]);
                                                        else if (getsIn)
                                                            this.Add(resultList, notification.NewItems[0]);
                                                    }
                                                    else
                                                    {
                                                        this
                                                            .RemoveRange(resultList, notification.OldItems.Where(x => filter(x)));

                                                        this
                                                            .AddRange(resultList, notification.NewItems.Where(x => filter(x)));
                                                    }

                                                    break;
                                                }

                                                default:
                                                {
                                                    this.Clear(resultList);
                                                    this.AddRange(resultList, notification.Current.Where(x => filter(x)));

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

            protected abstract void Add(TCollection collection, T item);
            protected abstract void AddRange(TCollection collection, IEnumerable<T> items);
            protected abstract void RemoveRange(TCollection collection, IEnumerable<T> items);
            protected abstract void Remove(TCollection collection, T oldItem);
            protected abstract void Replace(TCollection collection, T oldItem, T newItem);
            protected abstract void Clear(TCollection collection);

            public IObservable<TNotification> Changes { get; }
        }
        #endregion

        #region WhereReactiveList
        private sealed class WhereReactiveList<T> : WhereReactiveCollection<ListReactiveCollectionSource<T>, ListChangedNotification<T>, T>
        {
            private readonly IEqualityComparer<T> _equalityComparer;

            public WhereReactiveList(IObservable<ICollectionChangedNotification<T>> source, Predicate<T> filter, IEqualityComparer<T> equalityComparer) : base(source, filter)
            {
                Contract.Requires(source != null);
                Contract.Requires(filter != null);
                Contract.Requires(equalityComparer != null);

                this._equalityComparer = equalityComparer;
            }

            protected override void Add(ListReactiveCollectionSource<T> collection, T item)
            {
                collection.Add(item);
            }

            protected override void Clear(ListReactiveCollectionSource<T> collection)
            {
                collection.Clear();
            }

            protected override void AddRange(ListReactiveCollectionSource<T> collection, IEnumerable<T> items)
            {
                collection.AddRange(items);
            }

            protected override void Remove(ListReactiveCollectionSource<T> collection, T oldItem)
            {
                collection.Remove(oldItem, this._equalityComparer);
            }

            protected override void RemoveRange(ListReactiveCollectionSource<T> collection, IEnumerable<T> items)
            {
                collection.RemoveRange(items, this._equalityComparer);
            }

            protected override void Replace(ListReactiveCollectionSource<T> collection, T oldItem, T newItem)
            {
                collection.Replace(oldItem, newItem, this._equalityComparer);
            }
        }
        #endregion

        #region WhereReactiveDictionary
        private sealed class WhereReactiveDictionary<TKey, TValue> : WhereReactiveCollection<DictionaryReactiveCollectionSource<TKey, TValue>, DictionaryChangedNotification<TKey, TValue>, KeyValuePair<TKey, TValue>>
        {
            public WhereReactiveDictionary(IObservable<DictionaryChangedNotification<TKey, TValue>> source, Predicate<KeyValuePair<TKey, TValue>> filter) : base(source, filter)
            {
                Contract.Requires(source != null);
                Contract.Requires(filter != null);
            }

            protected override void RemoveRange(DictionaryReactiveCollectionSource<TKey, TValue> collection, IEnumerable<KeyValuePair<TKey, TValue>> items)
            {
                collection.RemoveRange(items.Select(x => x.Key));
            }

            protected override void Replace(DictionaryReactiveCollectionSource<TKey, TValue> collection, KeyValuePair<TKey, TValue> oldItem, KeyValuePair<TKey, TValue> newItem)
            {
                collection.Remove(oldItem.Key);
                collection.Add(newItem.Key, newItem.Value);
            }

            protected override void Add(DictionaryReactiveCollectionSource<TKey, TValue> collection, KeyValuePair<TKey, TValue> item)
            {
                collection.Add(item.Key, item.Value);
            }

            protected override void AddRange(DictionaryReactiveCollectionSource<TKey, TValue> collection, IEnumerable<KeyValuePair<TKey, TValue>> items)
            {
                collection.AddRange(items);
            }

            protected override void Clear(DictionaryReactiveCollectionSource<TKey, TValue> collection)
            {
                collection.Clear();
            }

            protected override void Remove(DictionaryReactiveCollectionSource<TKey, TValue> collection, KeyValuePair<TKey, TValue> oldItem)
            {
                collection.Remove(oldItem.Key);
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

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> Where<TKey, TValue>(this IReactiveCollection<DictionaryChangedNotification<TKey, TValue>> source, Predicate<KeyValuePair<TKey, TValue>> filter)
        {
            Contract.Requires(source != null);
            Contract.Requires(filter != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<DictionaryChangedNotification<TKey, TValue>>>() != null);

            return new WhereReactiveDictionary<TKey, TValue>(source.Changes, filter);
        }
    }
}
