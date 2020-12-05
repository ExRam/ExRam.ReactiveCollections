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
    public class DictionaryReactiveCollectionSource<TKey, TValue> : ReactiveCollectionSource<DictionaryChangedNotification<TKey, TValue>>,
        IDictionary<TKey, TValue>, 
        IDictionary,
        IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        public DictionaryReactiveCollectionSource() : this(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default)
        {

        }

        public DictionaryReactiveCollectionSource(IEqualityComparer<TKey> keyComparer) : this(keyComparer, EqualityComparer<TValue>.Default)
        {

        }

        public DictionaryReactiveCollectionSource(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer) : base(
            new DictionaryChangedNotification<TKey, TValue>(
                ImmutableDictionary<TKey, TValue>.Empty.WithComparers(keyComparer, valueComparer), 
                NotifyCollectionChangedAction.Reset, 
                ImmutableList<KeyValuePair<TKey, TValue>>.Empty, 
                ImmutableList<KeyValuePair<TKey, TValue>>.Empty))
        {
        }

        public void Add(TKey key, TValue value) => TryUpdate(notification => notification.Add(new KeyValuePair<TKey, TValue>(key, value)));

        public void AddRange(IEnumerable<TValue> values, Func<TValue, TKey> keySelector) => AddRange(values.Select(x => new KeyValuePair<TKey, TValue>(keySelector(x), x)));

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs) => TryUpdate(notification => notification.AddRange(pairs));

        public void Clear() => TryUpdate(notification => notification.Clear());

        public bool Contains(KeyValuePair<TKey, TValue> pair) => Current.Contains(pair);

        public void Remove(TKey key) => TryUpdate(notification => notification.Remove(key));

        public void RemoveRange(IEnumerable<TKey> keys)
        {
            // TODO: Optimize!
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public void SetItem(TKey key, TValue value) => TryUpdate(notification => notification.SetItem(key, value));

        public void SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items) => TryUpdate(notification => notification.SetItems(items));

        public bool ContainsKey(TKey key) => Current.ContainsKey(key);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) => Current.TryGetValue(key, out value);

        public TValue this[TKey key]
        {
            get => Current[key];
            set => SetItem(key, value);
        }

        public int Count => Current.Count;

        public IEnumerable<TValue> Values => Current.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Current.GetEnumerator();
        
        private ImmutableDictionary<TKey, TValue> Current => CurrentNotification.Current;

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
        void IDictionary.Add(object key, object? value) => throw new NotSupportedException();

        bool IDictionary.Contains(object key) => ContainsKey((TKey)key);

        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)Current).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => Current.Keys.ToList();

        void IDictionary.Remove(object key) => Remove((TKey)key);

        ICollection IDictionary.Values => Current.Values.ToList();

        object? IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index) => ((IDictionary)this).CopyTo(array, index);
        #endregion

        #region ICollection implementation
        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;
        #endregion

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IDictionary<TKey, TValue>)this).Keys;
    }
}
