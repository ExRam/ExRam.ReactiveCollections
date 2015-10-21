// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        public static IReactiveCollection<TNotification> WithChanges<TNotification>(this IReactiveCollection<TNotification> source, Func<IObservable<TNotification>, IObservable<TNotification>> changesTransformation) where TNotification : ICollectionChangedNotification
        {
            Contract.Requires(source != null);
            Contract.Requires(changesTransformation != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<TNotification>>() != null);

            return changesTransformation(source.Changes)
                .ToReactiveCollection();
        }
    }
}
