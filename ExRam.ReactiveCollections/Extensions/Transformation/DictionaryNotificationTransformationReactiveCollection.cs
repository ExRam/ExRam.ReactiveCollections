using System;
using System.Collections.Generic;

namespace ExRam.ReactiveCollections
{
    internal sealed class DictionaryNotificationTransformationReactiveCollection<TKey, TOriginalValue, TProjectedValue> : TransformationReactiveCollection<KeyValuePair<TKey, TOriginalValue>, KeyValuePair<TKey, TProjectedValue>, DictionaryReactiveCollectionSource<TKey, TProjectedValue>, DictionaryChangedNotification<TKey, TProjectedValue>>
    {
        public DictionaryNotificationTransformationReactiveCollection(IReactiveCollection<ICollectionChangedNotification<KeyValuePair<TKey, TOriginalValue>>> source, Predicate<KeyValuePair<TKey, TOriginalValue>> filter, Func<KeyValuePair<TKey, TOriginalValue>, KeyValuePair<TKey, TProjectedValue>> selector, IEqualityComparer<KeyValuePair<TKey, TProjectedValue>> equalityComparer) : base(source, new DictionaryReactiveCollectionSource<TKey, TProjectedValue>(), filter, selector, equalityComparer)
        {
        }
    }
}