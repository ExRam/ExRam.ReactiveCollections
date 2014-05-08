// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ExRam.ReactiveCollections
{
    public class ListReactiveCollectionSource<T> : 
        ReactiveCollectionSource<ListChangedNotification<T>, T>,
        IList<T>,
        IList
    {
        public ListReactiveCollectionSource() : this(ImmutableList<T>.Empty)
        {
        }

        public ListReactiveCollectionSource(IEnumerable<T> items)
        {
            Contract.Requires(items != null);

            this.Subject.OnNext(new ListChangedNotification<T>(ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null));

            if (!object.ReferenceEquals(items, ImmutableList<T>.Empty))
                this.AddRange(items);
        }

        public void Add(T item)
        {
            this.Insert(this.Current.Count, item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            this.InsertRange(this.Current.Count, items);
        }

        public void Clear()
        {
            this.Subject.OnNext(new ListChangedNotification<T>(ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null));
        }

        public bool Contains(T item)
        {
            return this.Current.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.Current.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Current.GetEnumerator();
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            var immutableItems = ImmutableList<T>.Empty.AddRange(items);

            if (immutableItems.Count > 0)
            {
                var newList = this.Current.InsertRange(index, immutableItems);
                this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, immutableItems, index));
            }
        }

        public int IndexOf(T item)
        {
            return this.Current.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Subject.OnNext(new ListChangedNotification<T>(this.Current.Insert(index, item), NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, ImmutableList.Create(item), index));
        }

        public bool Remove(T item)
        {
            return this.Remove(item, EqualityComparer<T>.Default);
        }

        public bool Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            var oldList = this.Current;
            var index = oldList.IndexOf(item, equalityComparer);
            if (index > -1)
                this.RemoveAt(index);

            return (this.Current != oldList);
        }

        public void RemoveAll(Predicate<T> match)
        {
            var newList = this.Current.RemoveAll(match);
            this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null));
        }

        public void RemoveAt(int index)
        {
            var oldList = this.Current;
            var oldItem = oldList[index];
            var newList = oldList.RemoveAt(index);
            this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(oldItem), ImmutableList<T>.Empty, index));
        }

        public void RemoveRange(int index, int count)
        {
            var oldList = this.Current;
            var range = oldList.GetRange(index, count);
            var newList = oldList.RemoveRange(index, count);
            this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Remove, range, ImmutableList<T>.Empty, index));
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            this.RemoveRange(items, EqualityComparer<T>.Default);
        }

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var removedItems = ImmutableList<T>.Empty.AddRange(items);

            if (removedItems.Count > 0)
            {
                if (removedItems.Count > 1)
                {
                    var newList = this.Current.RemoveRange(removedItems, equalityComparer);
                    this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null));
                }
                else
                    this.Remove(removedItems[0], equalityComparer);
            }
        }

        public void Replace(T oldValue, T newValue)
        {
            this.Replace(oldValue, newValue, EqualityComparer<T>.Default);
        }

        public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var index = this.Current.IndexOf(oldValue, 0, this.Count, equalityComparer);

            if (index > -1)
                this.SetItem(index, newValue);
        }

        public void Reverse()
        {
            this.Reverse(0, this.Count);
        }

        public void Reverse(int index, int count)
        {
            var newList = this.Current.Reverse(index, count);

            this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null));
        }

        public void SetItem(int index, T value)
        {
            var oldList = this.Current;
            var oldItem = oldList[index];
            var newList = oldList.SetItem(index, value);

            this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Replace, ImmutableList.Create(oldItem), ImmutableList.Create(value), index));
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
            var newList = this.Current.Sort(index, count, comparer);

            this.Subject.OnNext(new ListChangedNotification<T>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null));
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
            var oldList = this.Current;
            this.Insert(oldList.Count, (T)value);

            return oldList.Count;
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

        void IList<T>.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

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
                return this.Current.Count();
            }
        }

        private ImmutableList<T> Current
        {
            get
            {
                return this.Subject.Value.Current;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.Current[index];
            }

            set
            {
                this.SetItem(index, value);
            }
        }
    }
}
