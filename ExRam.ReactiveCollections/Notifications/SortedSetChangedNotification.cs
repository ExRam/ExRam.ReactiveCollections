using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace ExRam.ReactiveCollections
{
    public sealed class SortedSetChangedNotification<T> : CollectionChangedNotification<T>, IIndexedCollectionChangedNotification<T>
    {
        public static readonly SortedSetChangedNotification<T> Reset = new (ImmutableSortedSet<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);

        // ReSharper disable once SuggestBaseTypeForParameter
        public SortedSetChangedNotification(ImmutableSortedSet<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems, int? index) : base(current, action, oldItems, newItems)
        {
            Index = index;
        }

        public override ICollectionChangedNotification<T> ToResetNotification()
        {
            return new SortedSetChangedNotification<T>(Current, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public int? Index { get; }

        public new ImmutableSortedSet<T> Current => (ImmutableSortedSet<T>)base.Current;

        internal SortedSetChangedNotification<T> WithComparer(IComparer<T> comparer)
        {
            return new(Current.WithComparer(comparer), Action, OldItems, NewItems, Index);
        }
        
        internal SortedSetChangedNotification<T> Add(T value)
        {
            var current = Current;
            var newSet = current.Add(value);
            
            return newSet != current 
                ? new(newSet, NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, ImmutableList.Create(value), newSet.IndexOf(value)) 
                : this;
        }

        internal SortedSetChangedNotification<T> Clear()
        {
            var current = Current;

            return !current.IsEmpty
                ? new(current.Clear(), NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null)
                : this;
        }

        internal SortedSetChangedNotification<T> Except(IEnumerable<T> other)
        {
            var current = Current;
            var newSet = current.Except(other);

            return newSet != current 
                ? new(newSet, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null)
                : this;
        }

        internal SortedSetChangedNotification<T> Intersect(IEnumerable<T> other)
        {
            var current = Current;
            var newSet = current.Intersect(other);

            return newSet != current
                ? new(newSet, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null) 
                : this;
        }

        internal SortedSetChangedNotification<T> Remove(T value)
        {
            var current = Current;
            var newSet = current.Remove(value);

            return newSet != current
                ? new(newSet, NotifyCollectionChangedAction.Remove, ImmutableList.Create(value), ImmutableList<T>.Empty, Current.IndexOf(value))
                : this;
        }

        internal SortedSetChangedNotification<T> SymmetricExcept(IEnumerable<T> other)
        {
            var current = Current;
            var newSet = current.SymmetricExcept(other);

            return newSet != current
                ? new(newSet, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null) 
                : this;
        }

        internal SortedSetChangedNotification<T> Union(IEnumerable<T> other)
        {
            var current = Current;
            var newSet = current.Union(other);

            return newSet != current 
                ? new(newSet, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null)
                : this;
        }

        internal IEnumerable<SortedSetChangedNotification<T>> Sort(ICollectionChangedNotification<T> notification)
        {
            switch (notification.Action)
            {
                #region Add
                case NotifyCollectionChangedAction.Add:
                {
                    var current = this;
                    
                    foreach (var newItem in notification.NewItems)
                    {
                        current = current.Add(newItem);
                        
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
                        current = current.Remove(oldItem);

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
                        current = current.Remove(oldItem);

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
                            cleared = cleared.Add(newItem);

                            yield return cleared;
                        }
                    }

                    break;
                }
                #endregion
            }
        }
    }
}
