// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using ExRam.ReactiveCollections;
using JetBrains.Annotations;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region StateHolder
        private struct StateHolder<TNotification>
        {
            public readonly bool First;
            public readonly TNotification Notification;

            public StateHolder(bool first, TNotification notification)
            {
                First = first;
                Notification = notification;
            }
        }
        #endregion

        [NotNull]
        internal static IObservable<TNotification> Normalize<TNotification>([NotNull] this IObservable<TNotification> observable)
            where TNotification : ICollectionChangedNotification
        {
            return observable
                .Scan(new StateHolder<TNotification>(true, default(TNotification)), (state, notification) => new StateHolder<TNotification>(false, state.First ? (TNotification)notification.ToResetNotification() : notification))
                .Select(x => x.Notification);
        }
    }
}
