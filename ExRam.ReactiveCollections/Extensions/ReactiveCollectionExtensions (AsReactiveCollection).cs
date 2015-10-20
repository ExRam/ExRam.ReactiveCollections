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
        #region AsReactiveCollectionImpl
        private sealed class AsReactiveCollectionImpl<TNotification, T> : IReactiveCollection<TNotification>
            where TNotification : ICollectionChangedNotification<T>
        {
            private readonly IReactiveCollection<TNotification> _reactiveCollection;

            public AsReactiveCollectionImpl(IReactiveCollection<TNotification> reactiveCollection)
            {
                Contract.Requires(reactiveCollection != null);

                this._reactiveCollection = reactiveCollection;
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes
            {
                get
                {
                    return this._reactiveCollection.Changes.AsObservable();
                }
            }
        }
        #endregion

        public static IReactiveCollection<TNotification> AsReactiveCollection<TNotification, T>(this IReactiveCollection<TNotification> reactiveCollection)
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(reactiveCollection != null);
            Contract.Ensures(Contract.Result<IReactiveCollection<TNotification>>() != null);

            return new AsReactiveCollectionImpl<TNotification, T>(reactiveCollection);
        }
    }
}
