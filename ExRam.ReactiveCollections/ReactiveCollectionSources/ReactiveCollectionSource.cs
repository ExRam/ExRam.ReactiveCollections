// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExRam.ReactiveCollections
{
    public abstract class ReactiveCollectionSource<TNotification> : IReactiveCollectionSource<TNotification>
        where TNotification : ICollectionChangedNotification
    {
        #region ReactiveCollectionImpl
        private sealed class ReactiveCollectionImpl : IReactiveCollection<TNotification>
        {
            private readonly IObservable<TNotification> _changes;

            public ReactiveCollectionImpl(IObservable<TNotification> subject)
            {
                _changes = subject
                    .Normalize();
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => _changes;
        }
        #endregion

        protected ReactiveCollectionSource(TNotification initialNotification)
        {
            Subject = new BehaviorSubject<TNotification>(initialNotification);
            ReactiveCollection = new ReactiveCollectionImpl(Subject);
        }

        protected static bool IsCompatibleObject<T>(object? value)
        {
            // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
            return ((value is T) || (value == null && default(T) == null));
        }

        public IReactiveCollection<TNotification> ReactiveCollection { get; }

        protected BehaviorSubject<TNotification> Subject { get; }
    }
}
