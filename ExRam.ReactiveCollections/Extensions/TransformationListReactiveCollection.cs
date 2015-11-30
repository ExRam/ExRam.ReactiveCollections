using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    internal abstract class TransformationListReactiveCollection<TSource, TResult, TCollection, TNotification> : IReactiveCollection<TNotification>
        where TCollection : IReactiveCollectionSource<TNotification>, new()
        where TNotification : ICollectionChangedNotification
    {
        protected TransformationListReactiveCollection(IObservable<ICollectionChangedNotification<TSource>> source, Predicate<TSource> filter, Func<TSource, TResult> selector)
        {
            Contract.Requires(source != null);

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
                                    var localSelector = selector ?? (x => (TResult)(object)x);
                                    var listNotification = notification as ListChangedNotification<TSource>;

                                    lock (syncRoot)
                                    {
                                        switch (notification.Action)
                                        {
                                            case NotifyCollectionChangedAction.Add:
                                            {
                                                var insertionIndex = (filter == null) && listNotification != null
                                                    ? listNotification.Index
                                                    : null;

                                                var filteredItems = filter != null
                                                    ? notification.NewItems.Where(x => filter(x))
                                                    : notification.NewItems;

                                                this.InsertRange(resultList, insertionIndex, filteredItems.Select(localSelector));

                                                break;
                                            }

                                            case NotifyCollectionChangedAction.Remove:
                                            {
                                                if ((filter == null) && (listNotification?.Index != null))
                                                    this.RemoveRange(resultList, listNotification.Index.Value, notification.OldItems.Count);
                                                else
                                                {
                                                    var filtered = filter != null
                                                        ? notification.OldItems.Where(x => filter(x))
                                                        : notification.OldItems;

                                                    this.RemoveRange(resultList, filtered.Select(localSelector));
                                                }

                                                break;
                                            }

                                            case NotifyCollectionChangedAction.Replace:
                                            {
                                                var replaceIndex = (filter == null) && (listNotification != null)
                                                    ? listNotification.Index
                                                    : null;

                                                if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                {
                                                    var wasIn = filter?.Invoke(notification.OldItems[0]) ?? true;
                                                    var getsIn = filter?.Invoke(notification.NewItems[0]) ?? true;

                                                    if (wasIn && getsIn)
                                                        this.Replace(resultList, localSelector(notification.OldItems[0]), localSelector(notification.NewItems[0]));
                                                    else if (wasIn)
                                                        this.RemoveRange(resultList, notification.OldItems.Select(localSelector));
                                                    else if (getsIn)
                                                        this.InsertRange(resultList, null, notification.NewItems.Select(localSelector));
                                                }
                                                else
                                                {
                                                    if (replaceIndex.HasValue)
                                                    {
                                                        this
                                                            .RemoveRange(resultList, replaceIndex.Value, notification.OldItems.Count);

                                                        this
                                                            .InsertRange(resultList, replaceIndex.Value, notification.NewItems.Select(localSelector));
                                                    }
                                                    else
                                                    {
                                                        var removedItems = filter != null
                                                            ? notification.OldItems.Where(x => filter(x))
                                                            : notification.OldItems;

                                                        var addedItems = filter != null
                                                            ? notification.NewItems.Where(x => filter(x))
                                                            : notification.NewItems;

                                                        this
                                                            .RemoveRange(resultList, removedItems.Select(localSelector));

                                                        this
                                                            .InsertRange(resultList, null, addedItems.Select(localSelector));
                                                    }
                                                }

                                                break;
                                            }

                                            default:
                                            {
                                                this.Clear(resultList);

                                                var addedItems = filter != null
                                                    ? notification.Current.Where(x => filter(x))
                                                    : notification.Current;

                                                this.InsertRange(resultList, 0, addedItems.Select(localSelector));

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

        protected abstract void InsertRange(TCollection collection, int? index, IEnumerable<TResult> items);
        protected abstract void RemoveRange(TCollection collection, int index, int count);
        protected abstract void RemoveRange(TCollection collection, IEnumerable<TResult> items);
        protected abstract void Replace(TCollection collection, TResult oldItem, TResult newItem);
        protected abstract void Clear(TCollection collection);

        public IObservable<TNotification> Changes { get; }
    }
}
