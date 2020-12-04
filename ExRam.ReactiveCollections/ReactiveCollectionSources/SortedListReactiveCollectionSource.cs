// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExRam.ReactiveCollections
{
    public class SortedListReactiveCollectionSource<T> : 
        IReactiveCollectionSource<ListChangedNotification<T>>,
        ICollection<T>,
        ICollection,
        ICanReplaceValue<T>,
        ICanHandleRanges<T>
    {
        private readonly IComparer<T> _comparer;
        private readonly ListReactiveCollectionSource<T> _innerList = new();

        public SortedListReactiveCollectionSource() : this(Comparer<T>.Default)
        {
        }

        public SortedListReactiveCollectionSource(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public void Add(T item)
        {
            _innerList.Insert(FindInsertionIndex(item), item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (_innerList.Count == 0)
                _innerList.AddRange(items.OrderBy(x => x, _comparer));
            else
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Contains(T item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _innerList.IndexOf(item);
        }

        public bool Remove(T item)
        {
            return Remove(item, EqualityComparer<T>.Default);
        }

        public bool Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            return _innerList.Remove(item, equalityComparer);
        }

        public void RemoveAll(Predicate<T> match)
        {
            _innerList.RemoveAll(match);
        }

        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            _innerList.RemoveRange(index, count);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            RemoveRange(items, EqualityComparer<T>.Default);
        }

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> itemsEqualityComparer)
        {
            _innerList.RemoveRange(items, itemsEqualityComparer);
        }

        public void Replace(T oldValue, T newValue)
        {
            Replace(oldValue, newValue, EqualityComparer<T>.Default);
        }

        public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            Remove(oldValue, equalityComparer);
            Add(newValue);
        }

        #region Explicit ICollection implementation
        bool ICollection<T>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        #endregion

        private int FindInsertionIndex(T item)
        {
            // TODO: Optimize, do a binary search or something.
            for (var newInsertionIndex = 0; newInsertionIndex < _innerList.Count; newInsertionIndex++)
            {
                if (_comparer.Compare(item, _innerList[newInsertionIndex]) < 0)
                    return newInsertionIndex;
            }

            return _innerList.Count;
        }

        public int Count => _innerList.Count;

        public T this[int index] => _innerList[index];

        public IReactiveCollection<ListChangedNotification<T>> ReactiveCollection => _innerList.ReactiveCollection;
    }
}
