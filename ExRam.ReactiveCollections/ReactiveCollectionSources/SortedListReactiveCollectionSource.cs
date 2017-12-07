// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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
            if (this._innerList.Count == 0)
                this._innerList.AddRange(items.OrderBy(x => x, this._comparer));
            else
            {
                foreach (var item in items)
                {
                    this.Add(item);
                }
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

        public int IndexOf(T item)
        {
            return this._innerList.IndexOf(item);
        }

        public bool Remove(T item)
        {
            return this.Remove(item, EqualityComparer<T>.Default);
        }

        public bool Remove(T item, [NotNull] IEqualityComparer<T> equalityComparer)
        {
            return this._innerList.Remove(item, equalityComparer);
        }

        public void RemoveAll([NotNull] Predicate<T> match)
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

        public void RemoveRange([NotNull] IEnumerable<T> items)
        {
            this.RemoveRange(items, EqualityComparer<T>.Default);
        }

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> itemsequalityComparer)
        {
            this._innerList.RemoveRange(items);
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

        #region Explicit ICollection implementation
        bool ICollection<T>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo((T[])array, index);
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        #endregion

        private int FindInsertionIndex(T item)
        {
            // TODO: Optimize, do a binary search or something.
            for (var newInsertionIndex = 0; newInsertionIndex < this._innerList.Count; newInsertionIndex++)
            {
                if (this._comparer.Compare(item, this._innerList[newInsertionIndex]) < 0)
                    return newInsertionIndex;
            }

            return this._innerList.Count;
        }

        public int Count
        {
            get
            {
                return this._innerList.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                return this._innerList[index];
            }
        }

        public IReactiveCollection<ListChangedNotification<T>> ReactiveCollection => this._innerList.ReactiveCollection;
    }
}
