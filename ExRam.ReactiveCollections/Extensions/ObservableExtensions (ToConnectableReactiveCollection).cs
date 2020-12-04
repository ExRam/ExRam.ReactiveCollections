// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Reactive.Subjects;
using ExRam.ReactiveCollections;

namespace System.Reactive.Linq
{
    public static partial class ObservableExtensions
    {
        #region ToReactiveCollectionImpl
        private sealed class ToConnectableReactiveCollectionImpl<TNotification, T> : IConnectableReactiveCollection<TNotification>
            where TNotification : ICollectionChangedNotification<T>
        {
            private readonly Func<IDisposable> _connectFunction;
            private readonly IObservable<TNotification> _changes;

            public ToConnectableReactiveCollectionImpl(IObservable<TNotification> changes, Func<IDisposable> connectFunction)
            {
                _changes = changes
                    .Normalize();

                _connectFunction = connectFunction;
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => _changes;

            public IDisposable Connect()
            {
                return _connectFunction();
            }
        }
        #endregion

        public static IConnectableReactiveCollection<TNotification> ToConnectableReactiveCollection<TNotification, T>(this IObservable<TNotification> changesObservable, Func<IDisposable> connectFunction)
            where TNotification : ICollectionChangedNotification<T>
        {
            return new ToConnectableReactiveCollectionImpl<TNotification, T>(changesObservable, connectFunction);
        }

        public static IConnectableReactiveCollection<TNotification> ToConnectableReactiveCollection<TNotification, T>(this IConnectableObservable<TNotification> changesObservable)
           where TNotification : ICollectionChangedNotification<T>
        {
            return new ToConnectableReactiveCollectionImpl<TNotification, T>(changesObservable, changesObservable.Connect);
        }
    }
}
