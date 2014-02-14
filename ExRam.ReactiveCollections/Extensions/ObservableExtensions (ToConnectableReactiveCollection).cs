using System.Diagnostics.Contracts;
using ExRam.ReactiveCollections;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region ToReactiveCollectionImpl
        private sealed class ToConnectableReactiveCollectionImpl<TNotification, T> : IConnectableReactiveCollection<TNotification, T>
            where TNotification : ICollectionChangedNotification<T>
        {
            private readonly Func<IDisposable> _connectFunction;
            private readonly IObservable<TNotification> _changes;

            public ToConnectableReactiveCollectionImpl(IObservable<TNotification> changes, Func<IDisposable> connectFunction)
            {
                Contract.Requires(changes != null);
                Contract.Requires(connectFunction != null);

                this._changes = changes
                    .Normalize<TNotification, T>();

                this._connectFunction = connectFunction;
            }

            IObservable<TNotification> IReactiveCollection<TNotification, T>.Changes
            {
                get
                {
                    return this._changes;
                }
            }

            public IDisposable Connect()
            {
                return this._connectFunction();
            }
        }
        #endregion

        public static IConnectableReactiveCollection<TNotification, T> ToConnectableReactiveCollection<TNotification, T>(this IObservable<TNotification> changesObservable, Func<IDisposable> connectFunction)
            where TNotification : ICollectionChangedNotification<T>
        {
            Contract.Requires(changesObservable != null);
            Contract.Requires(connectFunction != null);

            return new ToConnectableReactiveCollectionImpl<TNotification, T>(changesObservable, connectFunction);
        }
    }
}
