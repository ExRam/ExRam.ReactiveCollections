using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;

namespace ExRam.ReactiveCollections
{
    public class ListChangedNotification<T> : CollectionChangedNotification<T>, IIndexedCollectionChangedNotification<T>
    {
        public static readonly ListChangedNotification<T> Reset = new (ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);

        // ReSharper disable once SuggestBaseTypeForParameter
        public ListChangedNotification(ImmutableList<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems, int? index) : base(current, action, oldItems, newItems)
        {
            Index = index;
        }

        public override ICollectionChangedNotification<T> ToResetNotification()
        {
            return new ListChangedNotification<T>(Current, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public int? Index { get; }

        public new ImmutableList<T> Current => (ImmutableList<T>)base.Current;
        
        internal ListChangedNotification<T> InsertRange(int index, IEnumerable<T> items)
        {
            var immutableItems = ImmutableList.CreateRange(items);

            if (immutableItems.IsEmpty)
                return this;

            var newList = Current.InsertRange(index, immutableItems);

            return newList != Current
                ? new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, immutableItems, index)
                : this;
        }

        internal ListChangedNotification<T> Sort(int index, int count, IComparer<T> comparer)
        {
            var newList = Current.Sort(index, count, comparer);

            return newList != Current 
                ? new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null)
                : this;
        }

        internal ListChangedNotification<T> SetItem(int index, T value)
        {
            var oldItem = Current[index];
            var newList = Current.SetItem(index, value);

            return Current != newList
                ? new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Replace, ImmutableList.Create(oldItem), ImmutableList.Create(value), index)
                : this;
        }

        internal ListChangedNotification<T> Reverse(int index, int count)
        {
            var newList = Current.Reverse(index, count);

            return newList != Current
                ? new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null) 
                : this;
        }

        internal ListChangedNotification<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var removedItems = ImmutableList.CreateRange(items);

            if (removedItems.Count > 0)
            {
                if (removedItems.Count > 1)
                {
                    var newList = Current.RemoveRange(removedItems, equalityComparer);
                    if (Current != newList)
                        return new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Remove, removedItems, ImmutableList<T>.Empty, null);
                }
                else
                    return Remove(removedItems[0], equalityComparer);
            }

            return this;
        }

        internal ListChangedNotification<T> RemoveRange(int index, int count)
        {
            var range = Current.GetRange(index, count);
            var newList = Current.RemoveRange(index, count);

            return newList != Current
                ? new (newList, NotifyCollectionChangedAction.Remove, range, ImmutableList<T>.Empty, index)
                : this;
        }

        internal ListChangedNotification<T> RemoveAt(int index)
        {
            var oldItem = Current[index];
            var newList = Current.RemoveAt(index);

            return Current != newList
                ? new (newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(oldItem), ImmutableList<T>.Empty, index) 
                : this;
        }

        internal ListChangedNotification<T> RemoveAll(Predicate<T> match)
        {
            var newList = Current.RemoveAll(match);
            return new (newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        internal ListChangedNotification<T> Insert(int index, T item)
        {
            return new (Current.Insert(index, item), NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, ImmutableList.Create(item), index);
        }

        internal ListChangedNotification<T> Clear()
        {
            return !Current.IsEmpty 
                ? new (ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null)
                : this;
        }

        internal ListChangedNotification<T> Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            var index = Current.IndexOf(item, equalityComparer);

            return index > -1
                ? RemoveAt(index) 
                : this;
        }

        internal IEnumerable<ListChangedNotification<T>> WhereSelect<TSource>(ICollectionChangedNotification<TSource> notification, Predicate<TSource>? filter, Func<TSource, T> selector, IEqualityComparer<T>? equalityComparer = null)
        {
            equalityComparer ??= EqualityComparer<T>.Default;
            
            switch (notification.Action)
            {
                #region Add
                case NotifyCollectionChangedAction.Add:
                {
                    var filteredItems = filter != null
                        ? notification.NewItems.Where(x => filter(x))
                        : notification.NewItems;

                    var selectedItems = filteredItems.Select(selector);

                    if (filter == null && notification is IIndexedCollectionChangedNotification<TSource> listChangedNotification && listChangedNotification?.Index != null)
                        yield return InsertRange(listChangedNotification.Index.Value, selectedItems);
                    else
                        yield return InsertRange(Current.Count, selectedItems);

                    break;
                }
                #endregion

                #region Remove
                case NotifyCollectionChangedAction.Remove:
                {
                    if (filter == null && notification is ListChangedNotification<TSource> listChangedNotification && listChangedNotification?.Index != null)
                        yield return RemoveRange(listChangedNotification.Index.Value, notification.OldItems.Count);
                    else
                    {
                        var filtered = filter != null
                            ? notification.OldItems.Where(x => filter(x))
                            : notification.OldItems;

                        yield return RemoveRange(filtered.Select(selector), equalityComparer);
                    }

                    break;
                }
                #endregion

                #region Replace
                case NotifyCollectionChangedAction.Replace:
                {
                    if (notification.OldItems.Count == 1 && notification.NewItems.Count == 1)
                    {
                        var wasIn = filter?.Invoke(notification.OldItems[0]) ?? true;
                        var getsIn = filter?.Invoke(notification.NewItems[0]) ?? true;

                        if (wasIn && getsIn)
                        {
                            var newItem = selector(notification.NewItems[0]);

                            if (filter == null && notification is IIndexedCollectionChangedNotification<TSource> listChangedNotification && listChangedNotification?.Index != null)
                                yield return SetItem(listChangedNotification.Index.Value, newItem);
                            else
                            {
                                var oldItem = selector(notification.OldItems[0]);
                                var index = Current.IndexOf(oldItem);

                                if (index > -1)
                                    yield return SetItem(index, newItem);
                                else
                                    yield return Insert(Current.Count, newItem);
                            }
                        }
                        else if (wasIn)
                            yield return RemoveRange(notification.OldItems.Select(selector), equalityComparer);
                        else if (getsIn)
                            yield return InsertRange(Current.Count, notification.NewItems.Select(selector));
                    }
                    else
                    {
                        if (filter == null && notification is IIndexedCollectionChangedNotification<TSource> listChangedNotification && listChangedNotification?.Index != null)
                        {
                            var removed = RemoveRange(listChangedNotification.Index.Value, notification.OldItems.Count);
                            
                            yield return removed;
                            yield return removed
                                .InsertRange(listChangedNotification.Index.Value, notification.NewItems.Select(selector));
                        }
                        else
                        {
                            var removedItems = filter != null
                                ? notification.OldItems.Where(x => filter(x))
                                : notification.OldItems;

                            var addedItems = filter != null
                                ? notification.NewItems.Where(x => filter(x))
                                : notification.NewItems;

                            var removed = RemoveRange(removedItems.Select(selector), equalityComparer);

                            yield return removed;
                            yield return removed.InsertRange(removed.Current.Count, addedItems.Select(selector));
                        }
                    }

                    break;
                }
                #endregion

                #region default
                default:
                {
                    var cleared = Clear();
                    
                    yield return cleared;

                    if (notification.Current.Count > 0)
                    {
                        var addedItems = filter != null
                            ? notification.Current.Where(x => filter(x))
                            : notification.Current;

                        yield return cleared.InsertRange(0, addedItems.Select(selector));
                    }

                    break;
                }
                #endregion
            }
        }
    }
}
