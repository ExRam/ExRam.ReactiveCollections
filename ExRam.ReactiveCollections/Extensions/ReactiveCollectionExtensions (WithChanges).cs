// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public static partial class ReactiveCollectionExtensions
    {
        [NotNull]
        public static IReactiveCollection<TNotification> WithChanges<TNotification>([NotNull] this IReactiveCollection<TNotification> source, [NotNull] Func<IObservable<TNotification>, IObservable<TNotification>> changesTransformation) where TNotification : ICollectionChangedNotification
        {
            return changesTransformation(source.Changes)
                .ToReactiveCollection();
        }
    }
}
