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
        #region AsReactiveCollectionImpl
        private sealed class AsReactiveCollectionImpl<TNotification, T> : IReactiveCollection<TNotification>
            where TNotification : ICollectionChangedNotification<T>
        {
            private readonly IReactiveCollection<TNotification> _reactiveCollection;

            public AsReactiveCollectionImpl([NotNull] IReactiveCollection<TNotification> reactiveCollection)
            {
                _reactiveCollection = reactiveCollection;
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => _reactiveCollection.Changes.AsObservable();
        }
        #endregion

        [NotNull]
        public static IReactiveCollection<TNotification> AsReactiveCollection<TNotification, T>([NotNull] this IReactiveCollection<TNotification> reactiveCollection)
            where TNotification : ICollectionChangedNotification<T>
        {
            return new AsReactiveCollectionImpl<TNotification, T>(reactiveCollection);
        }
    }
}
