using System.Diagnostics.Contracts;
using ExRam.ReactiveCollections;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region ToReactiveCollectionImpl
        private sealed class ToReactiveCollectionImpl<TNotification, T> : IReactiveCollection<TNotification, T>
            where TNotification : ICollectionChangedNotification<T>
        {
            private readonly IObservable<TNotification> _changes;

            public ToReactiveCollectionImpl(IObservable<TNotification> changes)
            {
                Contract.Requires(changes != null);

                this._changes = changes
                    .Normalize<TNotification, T>();
            }

            IObservable<TNotification> IReactiveCollection<TNotification, T>.Changes
            {
                get
                {
                    return this._changes;
                }
            }
        }
        #endregion

        public static IReactiveCollection<TNotification, T> ToReactiveCollection<TNotification, T>(this IObservable<TNotification> changesObservable)
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(changesObservable != null);

            return new ToReactiveCollectionImpl<TNotification, T>(changesObservable);
        }
    }
}
