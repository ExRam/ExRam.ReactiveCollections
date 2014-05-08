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
        public static IReactiveCollection<TNotification, TSource> WithChanges<TNotification, TSource>(this IReactiveCollection<TNotification, TSource> source, Func<IObservable<TNotification>, IObservable<TNotification>> changesTransformation) where TNotification : ICollectionChangedNotification<TSource>
        {
            Contract.Requires(source != null);
            Contract.Requires(changesTransformation != null);

            return changesTransformation(source.Changes)
                .ToReactiveCollection<TNotification, TSource>();
        }
    }
}
