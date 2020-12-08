using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace ExRam.ReactiveCollections
{
    public sealed class SortedListChangedNotification<T> : CollectionChangedNotification<T>, IIndexedCollectionChangedNotification<T>
    {
        public static readonly SortedListChangedNotification<T> Reset = new (ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null, Comparer<T>.Default);

        public SortedListChangedNotification(IReadOnlyCollection<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems, int? index, IComparer<T> comparer) : base(current, action, oldItems, newItems)
        {
            Index = index;
            Comparer = comparer;
        }
        
        public override ICollectionChangedNotification<T> ToResetNotification()
        {
            return new SortedListChangedNotification<T>(Current, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null, Comparer);
        }
        
        internal SortedListChangedNotification<T> WithComparer(IComparer<T> comparer)
        {
            return new(Current, Action, OldItems, NewItems, Index, comparer);
        }

        internal SortedListChangedNotification<T> Add(T item) => Insert(FindInsertionIndex(item), item);

        internal SortedListChangedNotification<T> Clear()
        {
            return !Current.IsEmpty 
                ? new (ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null, Comparer)
                : this;
        }
        
        internal SortedListChangedNotification<T> Insert(int index, T item)
        {
            return new(Current.Insert(index, item), NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, ImmutableList.Create(item), index, Comparer);
        }

        internal SortedListChangedNotification<T> Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            var index = Current.IndexOf(item, equalityComparer);

            if (index > -1)
            {
                var oldItem = Current[index];
                var newList = Current.RemoveAt(index);

                return Current != newList
                    ? new (newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(oldItem), ImmutableList<T>.Empty, index, Comparer)
                    : this;
            }
            
            return this;
        }

        internal SortedListChangedNotification<T> RemoveAt(int index)
        {
            var oldItem = Current[index];
            var newList = Current.RemoveAt(index);

            return Current != newList
                ? new(newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(oldItem), ImmutableList<T>.Empty, index, Comparer)
                : this;
        }

        internal SortedListChangedNotification<T> RemoveRange(int index, int count)
        {
            var range = Current.GetRange(index, count);
            var newList = Current.RemoveRange(index, count);

            return newList != Current
                ? new (newList, NotifyCollectionChangedAction.Remove, range, ImmutableList<T>.Empty, index, Comparer)
                : this;
        }

        internal SortedListChangedNotification<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var removedItems = ImmutableList.CreateRange(items);

            if (removedItems.Count > 0)
            {
                if (removedItems.Count > 1)
                {
                    var newList = Current.RemoveRange(removedItems, equalityComparer);
                    if (Current != newList)
                        return new (newList, NotifyCollectionChangedAction.Remove, removedItems, ImmutableList<T>.Empty, null, Comparer);
                }
                else
                    return Remove(removedItems[0], equalityComparer);
            }

            return this;
        }

        internal SortedListChangedNotification<T> RemoveAll(Predicate<T> match)
        {
            var newList = Current.RemoveAll(match);
            return new (newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null, Comparer);
        }

        internal IEnumerable<SortedListChangedNotification<T>> Sort(ICollectionChangedNotification<T> notification, IEqualityComparer<T> equalityComparer)
        {
            switch (notification.Action)
            {
                #region Add
                case NotifyCollectionChangedAction.Add:
                {
                    var current = this;

                    foreach (var newItem in notification.NewItems)
                    {
                        current = current.Add(newItem); //TODO
                        yield return current;
                    }

                    break;
                }
                #endregion

                #region Remove
                case NotifyCollectionChangedAction.Remove:
                {
                    var current = this;

                    foreach (var oldItem in notification.OldItems)
                    {
                        current = current.Remove(oldItem, equalityComparer);
                        yield return current;
                    }

                    break;
                }
                #endregion

                #region Replace
                case NotifyCollectionChangedAction.Replace:
                {
                    var current = this;

                    foreach (var oldItem in notification.OldItems)
                    {
                        current = current.Remove(oldItem, equalityComparer);
                        yield return current;
                    }

                    foreach (var newItem in notification.NewItems)
                    {
                        current = current.Add(newItem);
                        yield return current;
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
                        foreach (var newItem in notification.Current)
                        {
                            cleared = cleared.Add(newItem); //TODO
                            yield return cleared;
                        }
                    }

                    break;
                }
                #endregion
            }
        }
        
        private int FindInsertionIndex(T item)
        {
            // TODO: Optimize, do a binary search or something.
            for (var newInsertionIndex = 0; newInsertionIndex < Current.Count; newInsertionIndex++)
            {
                if (Comparer.Compare(item, Current[newInsertionIndex]) < 0)
                    return newInsertionIndex;
            }

            return Current.Count;
        }

        public int? Index { get; }
        
        public IComparer<T> Comparer { get; }

        public new ImmutableList<T> Current => (ImmutableList<T>)base.Current;
    }
}