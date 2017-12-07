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
using System.Linq;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public class DictionaryReactiveCollectionSource<TKey, TValue> :
        ReactiveCollectionSource<DictionaryChangedNotification<TKey, TValue>>,
        IDictionary<TKey, TValue>, 
        IDictionary,
        ICanHandleRanges<KeyValuePair<TKey, TValue>>
    {
        public DictionaryReactiveCollectionSource() : base(new DictionaryChangedNotification<TKey, TValue>(ImmutableDictionary<TKey, TValue>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty))
        {
        }

        #region IImmutableDictionary<TKey, TValue> implementation
        public void Add(TKey key, TValue value)
        {
            this.Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(this.Current.Add(key, value), NotifyCollectionChangedAction.Add, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, value))));
        }

        public void AddRange([NotNull] IEnumerable<TValue> values, [NotNull] Func<TValue, TKey> keySelector)
        {
            this.AddRange(values.Select(x => new KeyValuePair<TKey, TValue>(keySelector(x), x)));
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            var immutablePairs = ImmutableList.CreateRange(pairs);

            if (!immutablePairs.IsEmpty)
            {
                var current = this.Current;
                var newDict = current.AddRange(immutablePairs);

                if (newDict != current)
                    this.Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newDict, NotifyCollectionChangedAction.Add, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, immutablePairs));
            }
        }

        public void Clear()
        {
            if (!this.Current.IsEmpty)
                this.Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(ImmutableDictionary<TKey, TValue>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty));
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            return this.Current.Contains(pair);
        }

        public void Remove(TKey key)
        {
            var oldList = this.Current;
            var newList = oldList.Remove(key);

            if (oldList != newList)
            {
                var oldValue = oldList[key];
                this.Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, oldValue)), ImmutableList<KeyValuePair<TKey, TValue>>.Empty));
            }
        }

        public void RemoveRange([NotNull] IEnumerable<TKey> keys)
        {
            // TODO: Optimize!
            foreach (var key in keys)
            {
                this.Remove(key);
            }
        }

        public void SetItem(TKey key, TValue value)
        {
            var oldList = this.Current;
            var newList = oldList.SetItem(key, value);

            if (oldList != newList)
            {
                var oldValue = oldList[key];
                this.Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newList, NotifyCollectionChangedAction.Replace, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, oldValue)), ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, value))));
            }
        }

        public void SetItems([NotNull] IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var oldList = this.Current;
            var newList = oldList.SetItems(items);

            if (oldList != newList)
                this.Subject.OnNext(new DictionaryChangedNotification<TKey, TValue>(newList, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty));
        }

        public bool ContainsKey(TKey key)
        {
            return this.Current.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.Current.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.Current[key];
            }
            set
            {
                this.SetItem(key, value);
            }
        }

        public int Count => this.Current.Count;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.Current.GetEnumerator();
        }
        #endregion

        #region IDictionary<TKey, TValue> implementation
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.Add(key, value);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => this.Current.Keys.ToList();

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            var oldList = this.Current;
            this.Remove(key);

            return this.Current != oldList;
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values => this.Current.Values.ToList();

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> implementation
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this.Current).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)this).Remove(item.Key);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            this.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        #endregion

        #region IDictionary implementation
        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)this.Current).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => this.Current.Keys.ToList();

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        ICollection IDictionary.Values => this.Current.Values.ToList();

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }

            set
            {
                this[(TKey)key] = (TValue)value;
            }
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
            this.RemoveRange(items.Select(x => x.Key));
        }

        [NotNull]
        public IEnumerable<TValue> Values
        {
            get
            {
                return this.Current.Values;
            }
        }

        [NotNull]
        private ImmutableDictionary<TKey, TValue> Current
        {
            get
            {
                return this.Subject.Value.Current;
            }
        }
    }
}
