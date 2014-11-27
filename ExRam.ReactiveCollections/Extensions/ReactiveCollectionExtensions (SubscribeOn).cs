// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Diagnostics.Contracts;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<TNotification, TSource> SubscribeOn<TNotification, TSource>(this IReactiveCollection<TNotification, TSource> source, SynchronizationContext syncContext) where TNotification : ICollectionChangedNotification<TSource>
        {
            Contract.Requires(source != null);
            Contract.Requires(syncContext != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<TNotification, TSource>>() != null);

            return source.Changes
                .SubscribeOn(syncContext)
                .ToReactiveCollection<TNotification, TSource>();
        }

        public static IReactiveCollection<TNotification, TSource> SubscribeOn<TNotification, TSource>(this IReactiveCollection<TNotification, TSource> source, IScheduler scheduler) where TNotification : ICollectionChangedNotification<TSource>
        {
            Contract.Requires(source != null);
            Contract.Requires(scheduler != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<TNotification, TSource>>() != null);

            return source.Changes
                .SubscribeOn(scheduler)
                .ToReactiveCollection<TNotification, TSource>();
        }
    }
}
