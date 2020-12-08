using System;
using System.Collections;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    public class SortedListReactiveCollectionSource<T> :
        ReactiveCollectionSource<SortedListChangedNotification<T>>,
        ICollection<T>,
        ICollection
    {
        public SortedListReactiveCollectionSource() : this(Comparer<T>.Default)
        {
        }

        public SortedListReactiveCollectionSource(IComparer<T> comparer) : base(SortedListChangedNotification<T>.Reset.WithComparer(comparer))
        {
        }

        public void Add(T item) => TryUpdate(notification => notification.Add(item));

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear() => TryUpdate(notification => notification.Clear());

        public bool Contains(T item) => CurrentNotification.Current.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => CurrentNotification.Current.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => CurrentNotification.Current.GetEnumerator();

        public int IndexOf(T item) => CurrentNotification.Current.IndexOf(item);

        public bool Remove(T item) => Remove(item, EqualityComparer<T>.Default);

        public bool Remove(T item, IEqualityComparer<T> equalityComparer) => TryUpdate(notification => notification.Remove(item, equalityComparer));

        public void RemoveAll(Predicate<T> match) => TryUpdate(notification => notification.RemoveAll(match));

        public void RemoveAt(int index) => TryUpdate(notification => notification.RemoveAt(index));

        public void RemoveRange(int index, int count) => TryUpdate(notification => notification.RemoveRange(index, count));

        public void RemoveRange(IEnumerable<T> items) => RemoveRange(items, EqualityComparer<T>.Default);

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> itemsEqualityComparer) => TryUpdate(notification => notification.RemoveRange(items, itemsEqualityComparer));

        public void Replace(T oldValue, T newValue) => Replace(oldValue, newValue, EqualityComparer<T>.Default);

        public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            Remove(oldValue, equalityComparer);
            Add(newValue);
        }

        public int Count => CurrentNotification.Current.Count;

        public T this[int index] => CurrentNotification.Current[index];

        #region Explicit ICollection implementation
        bool ICollection<T>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
        #endregion
    }
}
