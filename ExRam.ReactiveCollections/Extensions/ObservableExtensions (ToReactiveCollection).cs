// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

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
                _changes = changes
                    .Normalize();
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => _changes;
        }
        #endregion

        public static IReactiveCollection<TNotification> ToReactiveCollection<TNotification>(this IObservable<TNotification> changesObservable)
            where TNotification : ICollectionChangedNotification
        {
            return new ToReactiveCollectionImpl<TNotification>(changesObservable);
        }
    }
}
