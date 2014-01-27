using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using ExRam.ReactiveCollections;

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
                this.First = first;
                this.Notification = notification;
            }
        }
        #endregion

        internal static IObservable<TNotification> Normalize<TNotification, T>(this IObservable<TNotification> observable)
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(observable != null);

            return observable
                .Scan(new StateHolder<TNotification>(true, default(TNotification)), (state, notification) => new StateHolder<TNotification>(false, ((state.First) ? ((TNotification)notification.ToResetNotification()) : (notification))))
                .Select(x => x.Notification)
                .DistinctUntilChanged(x => x.Current);
        }
    }
}
