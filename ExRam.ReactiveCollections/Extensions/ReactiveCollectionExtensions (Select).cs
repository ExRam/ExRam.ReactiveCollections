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
        #region SelectListReactiveCollection
        private sealed class SelectListReactiveCollection<TSource, TResult> : IReactiveCollection<ListChangedNotification<TResult>, TResult>
        {
            private readonly IObservable<ListChangedNotification<TResult>> _changes;

            public SelectListReactiveCollection(
                IObservable<ICollectionChangedNotification<TSource>> source, 
                Func<TSource, TResult> selector,
                IEqualityComparer<TResult> equalityComparer)
            {
                Contract.Requires(source != null);
                Contract.Requires(selector != null);
                Contract.Requires(equalityComparer != null);

                this._changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var resultList = new ListReactiveCollectionSource<TResult>();

                        return Observable.Using(
                            () => source.Subscribe(
                                (notification) =>
                                {
                                    lock (syncRoot)
                                    {
                                        switch (notification.Action)
                                        {
                                            case (NotifyCollectionChangedAction.Add):
                                            {
                                                var listNotification = notification as ListChangedNotification<TSource>;

                                                if (listNotification != null)
                                                    // ReSharper disable PossibleInvalidOperationException
                                                    resultList.InsertRange(listNotification.Index.Value, notification.NewItems.Select(selector));
                                                    // ReSharper restore PossibleInvalidOperationException
                                                else
                                                    resultList.AddRange(notification.NewItems.Select(selector));

                                                break;
                                            }

                                            case (NotifyCollectionChangedAction.Remove):
                                            {
                                                var listNotification = notification as ListChangedNotification<TSource>;

                                                if (listNotification != null)
                                                    // ReSharper disable PossibleInvalidOperationException
                                                    resultList.RemoveRange(listNotification.Index.Value, notification.OldItems.Count);
                                                    // ReSharper restore PossibleInvalidOperationException
                                                else
                                                    resultList.RemoveRange(notification.OldItems.Select(selector), equalityComparer);

                                                break;
                                            }

                                            case (NotifyCollectionChangedAction.Replace):
                                            {
                                                var listNotification = notification as ListChangedNotification<TSource>;

                                                if (listNotification != null)
                                                {
                                                    // ReSharper disable PossibleInvalidOperationException
                                                    var index = listNotification.Index.Value;
                                                    // ReSharper restore PossibleInvalidOperationException

                                                    if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                        resultList.Replace(selector(notification.OldItems[0]), selector(notification.NewItems[0]), equalityComparer);
                                                    else
                                                    {
                                                        resultList
                                                            .RemoveRange(index, notification.OldItems.Count);

                                                        resultList
                                                            .InsertRange(index, notification.NewItems.Select(selector));
                                                    }
                                                }
                                                else
                                                {
                                                    if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                        resultList.Replace(selector(notification.OldItems[0]), selector(notification.NewItems[0]), equalityComparer);
                                                    else
                                                    {
                                                        resultList
                                                            .RemoveRange(notification.OldItems.Select(selector), equalityComparer);

                                                        resultList
                                                            .AddRange(notification.NewItems.Select(selector));
                                                    }
                                                }

                                                break;
                                            }

                                            default:
                                            {
                                                resultList.Clear();
                                                resultList.AddRange(notification.Current.Select(selector));

                                                break;
                                            }
                                        }
                                    }
                                }),

                            _ => resultList.ReactiveCollection.Changes);
                    })
                    .Replay(1)
                    .RefCount()
                    .Normalize<ListChangedNotification<TResult>, TResult>();
            }

            public IObservable<ListChangedNotification<TResult>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        #region SelectListReactiveDictionarySource
        private sealed class SelectReactiveDictionarySource<TKey, TSource, TResult> : IReactiveCollection<DictionaryChangedNotification<TKey, TResult>, KeyValuePair<TKey, TResult>>
        {
            private readonly IObservable<DictionaryChangedNotification<TKey, TResult>> _changes;

            public SelectReactiveDictionarySource(
                IObservable<DictionaryChangedNotification<TKey, TSource>> source,
                Func<TSource, TResult> selector)
            {
                Contract.Requires(source != null);
                Contract.Requires(selector != null);

                this._changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var resultList = new DictionaryReactiveCollectionSource<TKey, TResult>();

                        return Observable.Using(
                            () => source
                                .Subscribe((notification) =>
                                {
                                    lock (syncRoot)
                                    {
                                        switch (notification.Action)
                                        {
                                            case (NotifyCollectionChangedAction.Add):
                                            {
                                                resultList.AddRange(notification.NewItems.Select(x => new KeyValuePair<TKey, TResult>(x.Key, selector(x.Value))));
                                                break;
                                            }

                                            case (NotifyCollectionChangedAction.Remove):
                                            {
                                                resultList.RemoveRange(notification.OldItems.Select(x => x.Key));
                                                break;
                                            }

                                            case (NotifyCollectionChangedAction.Replace):
                                            {
                                                resultList.RemoveRange(notification.OldItems.Select(x => x.Key));
                                                resultList.AddRange(notification.NewItems.Select(x => new KeyValuePair<TKey, TResult>(x.Key, selector(x.Value))));
                                             
                                                break;
                                            }

                                            default:
                                            {
                                                resultList.Clear();
                                                resultList.AddRange(notification.Current.Select(x => new KeyValuePair<TKey, TResult>(x.Key, selector(x.Value))));

                                                break;
                                            }
                                        }
                                    }
                                }),

                            _ => resultList.ReactiveCollection.Changes);
                    })
                    .Replay(1)
                    .RefCount()
                    .Normalize<DictionaryChangedNotification<TKey, TResult>, KeyValuePair<TKey, TResult>>();
            }

            public IObservable<DictionaryChangedNotification<TKey, TResult>> Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<ListChangedNotification<TResult>, TResult> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>, TSource> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TResult>, TResult>>() != null);

            return source.Select(selector, EqualityComparer<TResult>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TResult>, TResult> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>, TSource> source, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TResult>, TResult>>() != null);

            return new SelectListReactiveCollection<TSource, TResult>(source.Changes, selector, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>, KeyValuePair<TKey, TResult>> Select<TKey, TSource, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TSource>, KeyValuePair<TKey, TSource>> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<DictionaryChangedNotification<TKey, TResult>, KeyValuePair<TKey, TResult>>>() != null);

            return new SelectReactiveDictionarySource<TKey, TSource, TResult>(source.Changes, selector);
        }
    }
}
