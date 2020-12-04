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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ExRam.ReactiveCollections
{
    public class DictionaryReactiveCollectionSource<TKey, TValue> :
        ReactiveCollectionSource<DictionaryChangedNotification<TKey, TValue>>,
        IDictionary<TKey, TValue>, 
        IDictionary,
        IReadOnlyDictionary<TKey, TValue>,
        ICanHandleRanges<KeyValuePair<TKey, TValue>> where TKey : notnull
    {
        public DictionaryReactiveCollectionSource() : base(new DictionaryChangedNotification<TKey, TValue>(ImmutableDictionary<TKey, TValue>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty))
        {
        }

        #region IImmutableDictionary<TKey, TValue> implementation
        public void Add(TKey key, TValue value)
        {
            Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(Current.Add(key, value), NotifyCollectionChangedAction.Add, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, value))));
        }

        public void AddRange(IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
        {
            AddRange(values.Select(x => new KeyValuePair<TKey, TValue>(keySelector(x), x)));
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            var immutablePairs = ImmutableList.CreateRange(pairs);

            if (!immutablePairs.IsEmpty)
            {
                var current = Current;
                var newDict = current.AddRange(immutablePairs);

                if (newDict != current)
                    Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newDict, NotifyCollectionChangedAction.Add, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, immutablePairs));
            }
        }

        public void Clear()
        {
            if (!Current.IsEmpty)
                Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(ImmutableDictionary<TKey, TValue>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty));
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return Current.Contains(pair);
        }

        public void Remove(TKey key)
        {
            var oldList = Current;
            var newList = oldList.Remove(key);

            if (oldList != newList)
            {
                var oldValue = oldList[key];
                Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, oldValue)), ImmutableList<KeyValuePair<TKey, TValue>>.Empty));
            }
        }

        public void RemoveRange(IEnumerable<TKey> keys)
        {
            // TODO: Optimize!
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public void SetItem(TKey key, TValue value)
        {
            var oldList = Current;
            var newList = oldList.SetItem(key, value);

            if (oldList != newList)
            {
                var oldValue = oldList[key];
                Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newList, NotifyCollectionChangedAction.Replace, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, oldValue)), ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, value))));
            }
        }

        public void SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var oldList = Current;
            var newList = oldList.SetItems(items);

            if (oldList != newList)
                Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty));
        }

        public bool ContainsKey(TKey key)
        {
            return Current.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return Current.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => Current[key];
            set => SetItem(key, value);
        }

        public int Count => Current.Count;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Current.GetEnumerator();
        }
        #endregion

        #region IDictionary<TKey, TValue> implementation
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Current.Keys.ToList();

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var oldList = Current;
            Remove(key);

            return Current != oldList;
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Current.Values.ToList();
        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> implementation
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)Current).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)this).Remove(item.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        #endregion

        #region IDictionary implementation
        void IDictionary.Add(object key, object? value)
        {
            Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)Current).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => Current.Keys.ToList();

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        ICollection IDictionary.Values => Current.Values.ToList();

        object? IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IDictionary)this).CopyTo(array, index);
        }
        #endregion

        #region ICollection implementation
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        #endregion

        void ICanHandleRanges<KeyValuePair<TKey, TValue>>.RemoveRange(IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<KeyValuePair<TKey, TValue>> equalityComparer)
        {
            RemoveRange(items.Select(x => x.Key));
        }

        public IEnumerable<TValue> Values => Current.Values;

        private ImmutableDictionary<TKey, TValue> Current => Subject.Value.Current;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IDictionary<TKey, TValue>)this).Keys;
    }
}
