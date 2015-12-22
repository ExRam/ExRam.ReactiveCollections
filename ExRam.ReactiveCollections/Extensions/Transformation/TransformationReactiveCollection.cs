using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    internal abstract class TransformationReactiveCollection<TSource, TResult, TCollection, TNotification> : IReactiveCollection<TNotification>
        where TCollection : IReactiveCollectionSource<TNotification>, ICollection<TResult>, ICanHandleRanges<TResult>
        where TNotification : ICollectionChangedNotification<TResult>
    {
        protected TransformationReactiveCollection(
            IReactiveCollection<ICollectionChangedNotification<TSource>> source,
            TCollection collection,
            Predicate<TSource> filter,
            Func<TSource, TResult> selector,
            IEqualityComparer<TResult> equalityComparer)
        {
            Contract.Requires(source != null);
            Contract.Requires(equalityComparer != null);

            this.Source = source;
            this.Filter = filter;
            this.Selector = selector;
            this.EqualityComparer = equalityComparer;

            this.Changes = Observable
                .Defer(() =>
                {
                    var syncRoot = new object();
                    var isList = collection is IList<TResult>;
                    var canInsertAndRemoveRangesAtIndex = collection is ICanHandleIndexedRanges<TResult>;

                    return Observable
                        .Using(
                            () => source
                                .Changes
                                .Subscribe(
                                    notification =>
                                    {
                                        var localSelector = selector ?? (x => (TResult)(object)x);
                                        var listNotification = notification as IIndexedCollectionChangedNotification<TSource>;

                                        lock (syncRoot)
                                        {
                                            switch (notification.Action)
                                            {
                                                #region Add
                                                case NotifyCollectionChangedAction.Add:
                                                {
                                                    var filteredItems = filter != null
                                                        ? notification.NewItems.Where(x => filter(x))
                                                        : notification.NewItems;
                                                
                                                    var selectedItems = filteredItems.Select(localSelector);

                                                    if ((filter == null) && listNotification?.Index != null && canInsertAndRemoveRangesAtIndex)
                                                        ((ICanHandleIndexedRanges<TResult>)collection).InsertRange(listNotification.Index.Value, selectedItems);
                                                    else
                                                        collection.AddRange(selectedItems);

                                                    break;
                                                }
                                                #endregion

                                                #region Remove
                                                case NotifyCollectionChangedAction.Remove:
                                                {
                                                    if ((filter == null) && (listNotification?.Index != null) && canInsertAndRemoveRangesAtIndex)
                                                        ((ICanHandleIndexedRanges<TResult>)collection).RemoveRange(listNotification.Index.Value, notification.OldItems.Count);
                                                    else
                                                    {
                                                        var filtered = filter != null
                                                            ? notification.OldItems.Where(x => filter(x))
                                                            : notification.OldItems;

                                                        collection.RemoveRange(filtered.Select(localSelector), equalityComparer);
                                                    }

                                                    break;
                                                }
                                                #endregion

                                                #region Replace
                                                case NotifyCollectionChangedAction.Replace:
                                                {
                                                    if ((notification.OldItems.Count == 1) && (notification.NewItems.Count == 1))
                                                    {
                                                        var wasIn = filter?.Invoke(notification.OldItems[0]) ?? true;
                                                        var getsIn = filter?.Invoke(notification.NewItems[0]) ?? true;

                                                        if (wasIn && getsIn)
                                                        {
                                                            var newItem = localSelector(notification.NewItems[0]);
                                                        
                                                            if ((filter == null) && (listNotification?.Index != null) && isList)
                                                                ((IList<TResult>)collection)[listNotification.Index.Value] = newItem;
                                                            else
                                                            {
                                                                var oldItem = localSelector(notification.OldItems[0]);

                                                                var canReplace = collection as ICanReplaceValue<TResult>;
                                                                if (canReplace != null)
                                                                    canReplace.Replace(oldItem, newItem, equalityComparer);
                                                                else
                                                                {
                                                                    collection.Remove(oldItem);
                                                                    collection.Add(newItem);
                                                                }
                                                            }
                                                        }
                                                        else if (wasIn)
                                                            collection.RemoveRange(notification.OldItems.Select(localSelector), equalityComparer);
                                                        else if (getsIn)
                                                            collection.AddRange(notification.NewItems.Select(localSelector));
                                                    }
                                                    else
                                                    {
                                                        if ((filter == null) && (listNotification?.Index != null) && canInsertAndRemoveRangesAtIndex)
                                                        {
                                                            ((ICanHandleIndexedRanges<TResult>)collection).RemoveRange(listNotification.Index.Value, notification.OldItems.Count);
                                                            ((ICanHandleIndexedRanges<TResult>)collection).InsertRange(listNotification.Index.Value, notification.NewItems.Select(localSelector));
                                                        }
                                                        else
                                                        {
                                                            var removedItems = filter != null
                                                                ? notification.OldItems.Where(x => filter(x))
                                                                : notification.OldItems;

                                                            var addedItems = filter != null
                                                                ? notification.NewItems.Where(x => filter(x))
                                                                : notification.NewItems;

                                                            collection.RemoveRange(removedItems.Select(localSelector), equalityComparer);
                                                            collection.AddRange(addedItems.Select(localSelector));
                                                        }
                                                    }

                                                    break;
                                                }
                                                #endregion

                                                #region default
                                                default:
                                                {
                                                    collection.Clear();

                                                    var addedItems = filter != null
                                                        ? notification.Current.Where(x => filter(x))
                                                        : notification.Current;

                                                    collection.AddRange(addedItems.Select(localSelector));

                                                    break;
                                                }
                                                #endregion
                                            }
                                        }
                                    }),

                            _ => collection.ReactiveCollection.Changes);
                })
                .ReplayFresh(1)
                .RefCount()
                .Normalize();
        }

        public Predicate<TSource> Filter { get; }
        public Func<TSource, TResult> Selector { get; }
        public IObservable<TNotification> Changes { get; }
        public IEqualityComparer<TResult> EqualityComparer { get; }
        public IReactiveCollection<ICollectionChangedNotification<TSource>> Source { get; }
    }
}
