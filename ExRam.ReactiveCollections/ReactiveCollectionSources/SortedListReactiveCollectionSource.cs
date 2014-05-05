using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ExRam.ReactiveCollections
{
    public class SortedListReactiveCollectionSource<T> : 
        IReactiveCollectionSource<ListChangedNotification<T>, T>,
        IList<T>,
        IList
    {
        private readonly IComparer<T> _comparer;
        private readonly ListReactiveCollectionSource<T> _innerList = new ListReactiveCollectionSource<T>();

        public SortedListReactiveCollectionSource() : this(Comparer<T>.Default)
        {
        }

        public SortedListReactiveCollectionSource(IComparer<T> comparer)
        {
            this._comparer = comparer;
        }

        public void Add(T item)
        {
            this._innerList.Insert(this.FindInsertionIndex(item), item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            Contract.Requires(items != null);

            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        public void Clear()
        {
            this._innerList.Clear();
        }

        public bool Contains(T item)
        {
            return this._innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._innerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._innerList.GetEnumerator();
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(T item)
        {
            return this._innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            return this.Remove(item, EqualityComparer<T>.Default);
        }

        public bool Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            return this._innerList.Remove(item, equalityComparer);
        }

        public void RemoveAll(Predicate<T> match)
        {
            this._innerList.RemoveAll(match);
        }

        public void RemoveAt(int index)
        {
            this._innerList.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            this._innerList.RemoveRange(index, count);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            Contract.Requires(items != null);

            this.RemoveRange(items, EqualityComparer<T>.Default);
        }

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            Contract.Requires(items != null);

            foreach (var item in items)
            {
                this._innerList.Remove(item, equalityComparer);
            }
        }

        public void Replace(T oldValue, T newValue)
        {
            this.Replace(oldValue, newValue, EqualityComparer<T>.Default);
        }

        public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            this.Remove(oldValue, equalityComparer);
            this.Add(newValue);
        }

        public void Reverse()
        {
            this.Reverse(0, this.Count);
        }

        public void Reverse(int index, int count)
        {
            throw new NotSupportedException();
        }

        public void SetItem(int index, T value)
        {
            throw new NotSupportedException();
        }

        public void Sort()
        {
            this.Sort(Comparer<T>.Default);
        }

        public void Sort(Comparison<T> comparison)
        {
            Contract.Requires(comparison != null);

            this.Sort(comparison.ToComparer());
        }

        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, this.Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            throw new NotSupportedException();
        }

        private int FindInsertionIndex(T item)
        {
            //TODO: Optimize, do a binary search or something.
            for (var newInsertionIndex = 0; newInsertionIndex < this._innerList.Count; newInsertionIndex++)
            {
                if (this._comparer.Compare(item, this._innerList[newInsertionIndex]) < 0)
                    return newInsertionIndex;
            }

            return this._innerList.Count;
        }

        #region Explicit IList<T> implementation
        T IList<T>.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Explicit IList implementation
        int IList.Add(object value)
        {
            var insertionIndex = this.FindInsertionIndex((T)value);
            this._innerList.Insert(insertionIndex,(T)value);

            return insertionIndex;
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            return this.Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return this.IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void IList.Remove(object value)
        {
            this.Remove((T)value);
        }

        //void IList.RemoveAt(int index)
        //{
        //    this.RemoveAt(index);
        //}

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo((T[])array, index);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }
        #endregion

        public int Count
        {
            get
            {
                return this._innerList.Count();
            }
        }

        public T this[int index]
        {
            get
            {
                return this._innerList[index];
            }
        }

        public IReactiveCollection<ListChangedNotification<T>, T> ReactiveCollection
        {
            get
            {
                return this._innerList.ReactiveCollection;
            }
        }
    }
}
