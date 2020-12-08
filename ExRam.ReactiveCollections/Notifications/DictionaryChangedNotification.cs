using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;

namespace ExRam.ReactiveCollections
{
    public sealed class DictionaryChangedNotification<TKey, TValue> : CollectionChangedNotification<KeyValuePair<TKey, TValue>>
        where TKey: notnull
    {
        public static readonly DictionaryChangedNotification<TKey, TValue> Reset = new (ImmutableDictionary<TKey, TValue>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty);

        public DictionaryChangedNotification(
            ImmutableDictionary<TKey, TValue> current,
            NotifyCollectionChangedAction action,
            IReadOnlyList<KeyValuePair<TKey, TValue>> oldItems,
            IReadOnlyList<KeyValuePair<TKey, TValue>> newItems) : base(current, action, oldItems, newItems)
        {
            
        }

        public override ICollectionChangedNotification<KeyValuePair<TKey, TValue>> ToResetNotification() => new DictionaryChangedNotification<TKey, TValue>(Current, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty);

        public new ImmutableDictionary<TKey, TValue> Current => (ImmutableDictionary<TKey, TValue>)base.Current;

        public DictionaryChangedNotification<TKey, TValue> WithComparers(IEqualityComparer<TKey> keyComparer)
        {
            if (ReferenceEquals(Current.KeyComparer, keyComparer))
                return this;

            return new(Current.WithComparers(keyComparer), Action, OldItems, NewItems);
        }

        public DictionaryChangedNotification<TKey, TValue> WithComparers(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            if (ReferenceEquals(Current.KeyComparer, keyComparer))
                return this;

            return new(Current.WithComparers(keyComparer, valueComparer), Action, OldItems, NewItems);
        }

        internal DictionaryChangedNotification<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            var oldList = Current;
            var newList = oldList.SetItems(items);

            return oldList != newList 
                ? new (newList, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty) 
                : this;
        }

        internal DictionaryChangedNotification<TKey, TValue> SetItem(TKey key, TValue value)
        {
            var oldList = Current;
            var newList = oldList.SetItem(key, value);

            return oldList != newList
                ? new(newList, NotifyCollectionChangedAction.Replace, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, oldList[key])), ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, value)))
                : this; 
        }

        internal DictionaryChangedNotification<TKey, TValue> Remove(TKey key)
        {
            var newList = Current.Remove(key);

            return Current != newList 
                ? new(newList, NotifyCollectionChangedAction.Remove, ImmutableList.Create(new KeyValuePair<TKey, TValue>(key, Current[key])), ImmutableList<KeyValuePair<TKey, TValue>>.Empty)
                : this;
        }

        internal DictionaryChangedNotification<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
        {
            var newList = Current.RemoveRange(keys);
            var removed = keys
                .Select(key =>
                {
                    if (Current.TryGetValue(key, out var value))
                        return new KeyValuePair<TKey, TValue>(key, value);

                    return default(KeyValuePair<TKey, TValue>?);
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToImmutableList();
            
            return Current != newList
                ? new(newList, NotifyCollectionChangedAction.Remove, removed, ImmutableList<KeyValuePair<TKey, TValue>>.Empty)
                : this;
        }

        internal DictionaryChangedNotification<TKey, TValue> Clear()
        {
            return !Current.IsEmpty 
                ? new(ImmutableDictionary<TKey, TValue>.Empty, NotifyCollectionChangedAction.Reset, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList<KeyValuePair<TKey, TValue>>.Empty)
                : this;
        }

        internal DictionaryChangedNotification<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            var immutablePairs = ImmutableList.CreateRange(pairs);

            if (immutablePairs.IsEmpty)
                return this;
            
            var current = Current;
            var newDict = current.AddRange(immutablePairs);

            return newDict != current 
                ? new(newDict, NotifyCollectionChangedAction.Add, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, immutablePairs)
                : this;
        }

        internal DictionaryChangedNotification<TKey, TValue> Add(KeyValuePair<TKey, TValue> kvp)
        {
            return new(Current.Add(kvp.Key, kvp.Value),  NotifyCollectionChangedAction.Add, ImmutableList<KeyValuePair<TKey, TValue>>.Empty, ImmutableList.Create(kvp));
        }

        internal IEnumerable<DictionaryChangedNotification<TKey, TValue>> WhereSelect<TSourceValue>(DictionaryChangedNotification<TKey, TSourceValue> notification, Predicate<TSourceValue>? filter, Func<TSourceValue, TValue> selector)
        {
            switch (notification.Action)
            {
                #region Add
                case NotifyCollectionChangedAction.Add:
                {
                    var filteredItems = filter != null
                        ? notification.NewItems.Where(x => filter(x.Value))
                        : notification.NewItems;

                    var selectedItems = filteredItems
                        .Select(x => new KeyValuePair<TKey, TValue>(x.Key, selector(x.Value)));

                    yield return AddRange(selectedItems);

                    break;
                }
                #endregion

                #region Remove
                case NotifyCollectionChangedAction.Remove:
                {
                    yield return RemoveRange(notification.OldItems.Select(x => x.Key));
                    
                    break;
                }
                #endregion

                #region Replace
                case NotifyCollectionChangedAction.Replace:
                {
                    var filteredItems = filter != null
                        ? notification.NewItems.Where(x => filter(x.Value))
                        : notification.NewItems;
                    
                    var removed = RemoveRange(notification.OldItems
                        .Select(x => x.Key)); ;

                    yield return removed;
                    yield return removed.AddRange(filteredItems
                        .Select(x => new KeyValuePair<TKey, TValue>(x.Key, selector(x.Value))));
                    
                    break;
                }
                #endregion

                #region default
                default:
                {
                    var cleared = Clear()
                        .WithComparers(notification.Current.KeyComparer);

                    yield return cleared;

                    if (notification.Current.Count > 0)
                    {
                        var filteredItems = filter != null
                            ? notification.NewItems.Where(x => filter(x.Value))
                            : notification.NewItems;

                        yield return cleared.AddRange(filteredItems
                            .Select(x => new KeyValuePair<TKey, TValue>(x.Key, selector(x.Value))));
                    }

                    break;
                }
                #endregion
            }
        }
    }
}
