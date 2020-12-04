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

namespace ExRam.ReactiveCollections
{
    public class ListReactiveCollectionSource<T> : 
        ReactiveCollectionSource<ListChangedNotification<T>>,
        IList<T>,
        IList,
        ICanReplaceValue<T>,
        ICanHandleIndexedRanges<T>,
        ICanHandleRanges<T>
    {
        public ListReactiveCollectionSource() : this(ImmutableList<T>.Empty)
        {
        }

        public ListReactiveCollectionSource(IEnumerable<T> items) : base(new ListChangedNotification<T>(ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null))
        {
            if (!ReferenceEquals(items, ImmutableList<T>.Empty))
                AddRange(items);
        }

        public void Add(T item) => Insert(Current.Count, item);

        public void AddRange(IEnumerable<T> items) => InsertRange(Current.Count, items);

        public void Clear()
        {
            if (!Current.IsEmpty)
                OnNext(ImmutableList<T>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public bool Contains(T item) => Current.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => Current.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => Current.GetEnumerator();

        public void InsertRange(int index, IEnumerable<T> items)
        {
            var immutableItems = ImmutableList.CreateRange(items);

            if (!immutableItems.IsEmpty)
            {
                var current = Current;
                var newList = current.InsertRange(index, immutableItems);

                if (newList != current)
                    OnNext(newList, NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, immutableItems, index);
            }
        }

        public int IndexOf(T item) => Current.IndexOf(item);

        public void Insert(int index, T item)
        {
            OnNext(Current.Insert(index, item), NotifyCollectionChangedAction.Add, ImmutableList<T>.Empty, ImmutableList.Create(item), index);
        }

        public bool Remove(T item) => Remove(item, EqualityComparer<T>.Default);

        public bool Remove(T item, IEqualityComparer<T> equalityComparer)
        {
            var oldList = Current;
            var index = oldList.IndexOf(item, equalityComparer);

            return index > -1 && RemoveAtInternal(index);
        }

        public void RemoveAll(Predicate<T> match)
        {
            var newList = Current.RemoveAll(match);
            OnNext(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public void RemoveAt(int index) => RemoveAtInternal(index);

        private bool RemoveAtInternal(int index)
        {
            var oldList = Current;
            var oldItem = oldList[index];
            var newList = oldList.RemoveAt(index);

            if (oldList != newList)
            {
                OnNext(newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(oldItem), ImmutableList<T>.Empty, index);
                return true;
            }

            return false;
        }

        public void RemoveRange(int index, int count)
        {
            var oldList = Current;
            var range = oldList.GetRange(index, count);
            var newList = oldList.RemoveRange(index, count);

            if (newList != oldList)
                OnNext(newList, NotifyCollectionChangedAction.Remove, range, ImmutableList<T>.Empty, index);
        }

        public void RemoveRange(IEnumerable<T> items) => RemoveRange(items, EqualityComparer<T>.Default);

        public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            var removedItems = ImmutableList.CreateRange(items);

            if (removedItems.Count > 0)
            {
                if (removedItems.Count > 1)
                {
                    var current = Current;
                    var newList = current.RemoveRange(removedItems, equalityComparer);
                    if (current != newList)
                       OnNext(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
                }
                else
                    Remove(removedItems[0], equalityComparer);
            }
        }

        public void Replace(T oldValue, T newValue) => Replace(oldValue, newValue, EqualityComparer<T>.Default);

        public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            var index = Current.IndexOf(oldValue, 0, Count, equalityComparer);

            if (index > -1)
                SetItem(index, newValue);
        }

        public void Reverse() => Reverse(0, Count);

        public void Reverse(int index, int count)
        {
            var current = Current;
            var newList = current.Reverse(index, count);

            if (newList != current)
                OnNext(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public void SetItem(int index, T value)
        {
            var oldList = Current;
            var oldItem = oldList[index];
            var newList = oldList.SetItem(index, value);

            if (oldList != newList)
                OnNext(newList, NotifyCollectionChangedAction.Replace, ImmutableList.Create(oldItem), ImmutableList.Create(value), index);
        }

        public void Sort() => Sort(Comparer<T>.Default);

        public void Sort(Comparison<T> comparison) => Sort(comparison.ToComparer());

        public void Sort(IComparer<T> comparer) => Sort(0, Count, comparer);

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            var current = Current;
            var newList = current.Sort(index, count, comparer);

            if (newList != current)
                OnNext(newList, NotifyCollectionChangedAction.Reset, ImmutableList<T>.Empty, ImmutableList<T>.Empty, null);
        }

        public T this[int index]
        {
            get => Current[index];
            set => SetItem(index, value);
        }
        
        public int Count => Current.Count;

        private ImmutableList<T> Current => Subject.Value.Current;

        private void OnNext(ImmutableList<T> newList, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems, int? index) => Subject.OnNext(new ListChangedNotification<T>(newList, action, oldItems, newItems, index));

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
