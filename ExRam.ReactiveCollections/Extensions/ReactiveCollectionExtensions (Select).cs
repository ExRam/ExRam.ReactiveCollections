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
        #region SelectReactiveDictionarySource
        private sealed class SelectReactiveDictionarySource<TKey, TSource, TResult> : IReactiveCollection<DictionaryChangedNotification<TKey, TResult>>
        {
            public SelectReactiveDictionarySource(
                IObservable<DictionaryChangedNotification<TKey, TSource>> source,
                Func<TSource, TResult> selector)
            {
                Contract.Requires(source != null);
                Contract.Requires(selector != null);

                this.Changes = Observable
                    .Defer(() =>
                    {
                        var syncRoot = new object();
                        var resultList = new DictionaryReactiveCollectionSource<TKey, TResult>();

                        return Observable
                            .Using(
                                () => source
                                    .Subscribe(notification =>
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
                    .ReplayFresh(1)
                    .RefCount()
                    .Normalize();
            }

            public IObservable<DictionaryChangedNotification<TKey, TResult>> Changes { get; }
        }
        #endregion

        public static IReactiveCollection<ListChangedNotification<TResult>> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TResult>>>() != null);

            return source.Select(selector, EqualityComparer<TResult>.Default);
        }

        public static IReactiveCollection<ListChangedNotification<TResult>> Select<TSource, TResult>(this IReactiveCollection<ICollectionChangedNotification<TSource>> source, Func<TSource, TResult> selector, IEqualityComparer<TResult> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Requires(equalityComparer != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<ListChangedNotification<TResult>>>() != null);

            var transformedReactiveCollection = source as ListNotificationTransformationListReactiveCollection<TSource, TSource>;
            if (transformedReactiveCollection != null && transformedReactiveCollection.CanAddSelect())
                return transformedReactiveCollection.AddSelect(selector, equalityComparer);

            return new ListNotificationTransformationListReactiveCollection<TSource, TResult>(source, null, selector, equalityComparer);
        }

        public static IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> Select<TKey, TSource, TResult>(this IReactiveCollection<DictionaryChangedNotification<TKey, TSource>> source, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);
            Contract.Requires(selector != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<DictionaryChangedNotification<TKey, TResult>>>() != null);

            return new SelectReactiveDictionarySource<TKey, TSource, TResult>(source.Changes, selector);
        }
    }
}
