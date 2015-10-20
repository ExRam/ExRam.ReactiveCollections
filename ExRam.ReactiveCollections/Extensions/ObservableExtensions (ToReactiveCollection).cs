// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Diagnostics.Contracts;
using ExRam.ReactiveCollections;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region ToReactiveCollectionImpl
        private sealed class ToReactiveCollectionImpl<TNotification> : IReactiveCollection<TNotification>
            where TNotification : ICollectionChangedNotification
        {
            private readonly IObservable<TNotification> _changes;

            public ToReactiveCollectionImpl(IObservable<TNotification> changes)
            {
                Contract.Requires(changes != null);

                this._changes = changes
                    .Normalize();
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<TNotification> ToReactiveCollection<TNotification>(this IObservable<TNotification> changesObservable)
            where TNotification : ICollectionChangedNotification
        {
            Contract.Requires(changesObservable != null);

            return new ToReactiveCollectionImpl<TNotification>(changesObservable);
        }
    }
}
