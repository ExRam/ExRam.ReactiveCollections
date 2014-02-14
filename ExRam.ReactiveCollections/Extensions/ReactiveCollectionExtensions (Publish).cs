using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IConnectableReactiveCollection<TNotification, T> Publish<TNotification, T>(this IReactiveCollection<TNotification, T> source) 
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(source != null);

            var changes = source
                .Changes
                .Publish();

            return changes
                .ToConnectableReactiveCollection<TNotification, T>(changes.Connect);
        }
    }
}
