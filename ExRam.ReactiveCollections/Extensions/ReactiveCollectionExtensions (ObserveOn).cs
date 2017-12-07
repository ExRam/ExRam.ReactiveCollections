// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        [NotNull]
        public static IReactiveCollection<TNotification> ObserveOn<TNotification, TSource>([NotNull] this IReactiveCollection<TNotification> source, [NotNull] SynchronizationContext syncContext) where TNotification : ICollectionChangedNotification<TSource>
        {
            return source.Changes
                .ObserveOn(syncContext)
                .ToReactiveCollection();
        }

        [NotNull]
        public static IReactiveCollection<TNotification> ObserveOn<TNotification, TSource>([NotNull] this IReactiveCollection<TNotification> source, [NotNull] IScheduler scheduler) where TNotification : ICollectionChangedNotification<TSource>
        {
            return source.Changes
                .ObserveOn(scheduler)
                .ToReactiveCollection();
        }
    }
}
