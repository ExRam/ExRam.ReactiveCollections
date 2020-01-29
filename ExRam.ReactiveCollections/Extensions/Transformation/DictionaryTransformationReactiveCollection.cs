using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal sealed class DictionaryTransformationReactiveCollection<TKey, TOriginalValue, TProjectedValue> : TransformationReactiveCollection<KeyValuePair<TKey, TOriginalValue>, KeyValuePair<TKey, TProjectedValue>, DictionaryReactiveCollectionSource<TKey, TProjectedValue>, DictionaryChangedNotification<TKey, TProjectedValue>>, ICanProjectDictionary<TKey, TProjectedValue>
    {
        public DictionaryTransformationReactiveCollection(IReactiveCollection<ICollectionChangedNotification<KeyValuePair<TKey, TOriginalValue>>> source, Predicate<KeyValuePair<TKey, TOriginalValue>> filter, Func<KeyValuePair<TKey, TOriginalValue>, KeyValuePair<TKey, TProjectedValue>> selector, IEqualityComparer<KeyValuePair<TKey, TProjectedValue>> equalityComparer) : base(source, new DictionaryReactiveCollectionSource<TKey, TProjectedValue>(), filter, selector, equalityComparer)
        {
        }

        public IReactiveCollection<DictionaryChangedNotification<TKey, TResult>> Select<TResult>(Func<TProjectedValue, TResult> selector)
        {
            return new DictionaryTransformationReactiveCollection<TKey, TOriginalValue, TResult>(
               Source,
               Filter,
               Selector != null
                   ? (Func<KeyValuePair<TKey, TOriginalValue>, KeyValuePair<TKey, TResult>>)(kvp => new KeyValuePair<TKey, TResult>(kvp.Key, selector(Selector(kvp).Value)))
                   : (kvp => new KeyValuePair<TKey, TResult>(kvp.Key, selector((TProjectedValue)(object)kvp.Value))),
               EqualityComparer<KeyValuePair<TKey, TResult>>.Default);
        }
    }
}