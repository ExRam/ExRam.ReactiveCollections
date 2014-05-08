// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReadOnlyCollection<T> GetCurrent<TNotification, T>(this IReactiveCollection<TNotification, T> reactiveCollection)
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(reactiveCollection != null);
            Contract.Ensures(Contract.Result<IReadOnlyCollection<T>>() != null);

            return reactiveCollection.Changes
                .FirstAsync()
                .ToTask()
                .Result
                .Current;
        }

        public static async Task<IReadOnlyCollection<T>> GetCurrentAsync<TNotification, T>(this IReactiveCollection<TNotification, T> reactiveCollection)
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(reactiveCollection != null);

            return (await reactiveCollection.Changes
                .FirstAsync()
                .ToTask()).Current;
        }
    }
}
