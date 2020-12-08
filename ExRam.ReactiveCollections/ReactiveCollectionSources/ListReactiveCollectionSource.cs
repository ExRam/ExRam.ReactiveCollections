using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ExRam.ReactiveCollections
{
    public class ListReactiveCollectionSource<T> : 
        ReactiveCollectionSource<ListChangedNotification<T>>,
        IList<T>,
        IList
    {
        public ListReactiveCollectionSource() : this(ImmutableList<T>.Empty)
        {
        }

        public ListReactiveCollectionSource(IEnumerable<T> items) : base(ListChangedNotification<T>.Reset)
        {
            if (!ReferenceEquals(items, ImmutableList<T>.Empty))
                AddRange(items);
        }

        public void Add(T item) => Insert(Current.Count, item);

        public void AddRange(IEnumerable<T> items) => InsertRange(Current.Count, items);

        public void Clear() => TryUpdate(notification => notification.Clear());

        public bool Contains(T item) => Current.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => Current.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => Current.GetEnumerator();

        public void InsertRange(int index, IEnumerable<T> items) => TryUpdate(notification => notification.InsertRange(index, items));

        public int IndexOf(T item) => Current.IndexOf(item);

        public void Insert(int index, T item) => TryUpdate(notification => notification.Insert(index, item));

        public bool Remove(T item) => Remove(item, EqualityComparer<T>.Default);

        public bool Remove(T item, IEqualityComparer<T> equalityComparer) => TryUpdate(notification => notification.Remove(item, equalityComparer));

        public void RemoveAll(Predicate<T> match) => TryUpdate(notification => notification.RemoveAll(match));

        public void RemoveAt(int index) => TryUpdate(notification => notification.RemoveAt(index));

        public void RemoveRange(int index, int count) => TryUpdate(notification => notification.RemoveRange(index, count));

        public void RemoveRange(IEnumerable<T> items) => RemoveRange(items, EqualityComparer<T>.Default);

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer) => TryUpdate(notification => notification.RemoveRange(items, equalityComparer));

        public void Replace(T oldValue, T newValue) => Replace(oldValue, newValue, EqualityComparer<T>.Default);

        public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var index = Current.IndexOf(oldValue, 0, Count, equalityComparer);

            if (index > -1)
                SetItem(index, newValue);
        }

        public void Reverse() => Reverse(0, Count);

        public void Reverse(int index, int count) => TryUpdate(notification => notification.Reverse(index, count));

        public void SetItem(int index, T value) => TryUpdate(notification => notification.SetItem(index, value));

        public void Sort() => Sort(Comparer<T>.Default);

        public void Sort(Comparison<T> comparison) => Sort(comparison.ToComparer());

        public void Sort(IComparer<T> comparer) => Sort(0, Count, comparer);

        public void Sort(int index, int count, IComparer<T> comparer) => TryUpdate(notification => notification.Sort(index, count, comparer));

        public T this[int index]
        {
            get => Current[index];
            set => SetItem(index, value);
        }
        
        public int Count => Current.Count;

        private ImmutableList<T> Current => CurrentNotification.Current;

        #region Explicit IList<T> implementation
        T IList<T>.this[int index]
        {
            get => this[index];
            set => SetItem(index, value);
        }

        bool ICollection<T>.IsReadOnly => false;
        #endregion

        #region Explicit IList implementation
        int IList.Add(object? value) => throw new NotSupportedException();

        bool IList.Contains(object? value) => IsCompatibleObject<T>(value) && Contains((T)value!);

        int IList.IndexOf(object? value) => IsCompatibleObject<T>(value) ? IndexOf((T)value!) : -1;

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        void IList.Remove(object? value) => throw new NotSupportedException();

        void IList<T>.RemoveAt(int index) => RemoveAt(index);

        object? IList.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
        #endregion
    }
}
