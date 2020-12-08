using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;

namespace ExRam.ReactiveCollections
{
    public class SortedSetReactiveCollectionSource<T> :
        ReactiveCollectionSource<SortedSetChangedNotification<T>>,
        IList<T>,
        IList,
        ISet<T>
    {
        public SortedSetReactiveCollectionSource() : this(Comparer<T>.Default)
        {
        }

        public SortedSetReactiveCollectionSource(IComparer<T> comparer) : base(new SortedSetChangedNotification<T>(ImmutableSortedSet.Create(comparer), NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null))
        {
            
        }

        public void Add(T value) => TryUpdate(notification => notification.Add(value));

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear() => TryUpdate(notification => notification.Clear());

        public void Except(IEnumerable<T> other) => TryUpdate(notification => notification.Except(other));

        public void Intersect(IEnumerable<T> other) => TryUpdate(notification => notification.Intersect(other));

        public bool IsProperSubsetOf(IEnumerable<T> other) => Current.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => Current.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => Current.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => Current.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => Current.Overlaps(other);

        public void Remove(T value) => TryUpdate(notification => notification.Remove(value));

        public bool SetEquals(IEnumerable<T> other) => Current.SetEquals(other);

        public void SymmetricExcept(IEnumerable<T> other) => TryUpdate(notification => notification.SymmetricExcept(other));

        public bool TryGetValue(T equalValue, out T actualValue) => Current.TryGetValue(equalValue, out actualValue);

        public void Union(IEnumerable<T> other) => TryUpdate(notification => notification.Union(other));

        private ImmutableSortedSet<T> Current => CurrentNotification.Current;

        #region IList<T> implementation
        public int IndexOf(T item) => Current.IndexOf(item);

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

        void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

        public T this[int index]
        {
            get => Current[index];
            set => throw new NotSupportedException();
        }
        #endregion

        #region ICollection<T> implementation
        public bool Contains(T item) => Current.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => ((ICollection<T>)Current).CopyTo(array, arrayIndex);

        public int Count => Current.Count;

        bool ICollection<T>.IsReadOnly => false;

        bool ICollection<T>.Remove(T item)
        {
            var oldList = Current;
            Remove(item);

            return Current != oldList;
        }
        #endregion

        #region IEnumerable<T> implemenation
        public IEnumerator<T> GetEnumerator()
        {
            return Current.GetEnumerator();
        }
        #endregion

        #region IList implementation
        int IList.Add(object? value) => throw new NotSupportedException();

        void IList.Clear() => Clear();

        bool IList.Contains(object? value) => Contains((T)value!);

        int IList.IndexOf(object? value) => IndexOf((T)value!);

        void IList.Insert(int index, object? value) => throw new NotSupportedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        void IList.Remove(object? value) => throw new NotSupportedException();

        void IList.RemoveAt(int index) => throw new NotSupportedException();

        object? IList.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
        #endregion

        #region ISet implementation
        void ISet<T>.ExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

        void ISet<T>.IntersectWith(IEnumerable<T> other) => throw new NotSupportedException();

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

        void ISet<T>.UnionWith(IEnumerable<T> other) => throw new NotSupportedException();

        bool ISet<T>.Add(T item) => throw new NotSupportedException();
        #endregion
    }
}
