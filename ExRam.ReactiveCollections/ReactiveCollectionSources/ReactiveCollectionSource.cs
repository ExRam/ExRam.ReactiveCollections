// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace ExRam.ReactiveCollections
{
    public abstract class ReactiveCollectionSource<TNotification> : IReactiveCollectionSource<TNotification>
        where TNotification : ICollectionChangedNotification
    {
        #region ReactiveCollectionImpl
        private sealed class ReactiveCollectionImpl : IReactiveCollection<TNotification>
        {
            private readonly IObservable<TNotification> _changes;

            public ReactiveCollectionImpl([NotNull] IObservable<TNotification> subject)
            {
                this._changes = subject
                    .Normalize();
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => this._changes;
        }
        #endregion

        protected ReactiveCollectionSource([NotNull] TNotification initialNotification)
        {
            this.Subject = new BehaviorSubject<TNotification>(initialNotification);
            this.ReactiveCollection = new ReactiveCollectionImpl(this.Subject);
        }

        [NotNull]
        public IReactiveCollection<TNotification> ReactiveCollection { get; }

        [NotNull]
        protected BehaviorSubject<TNotification> Subject { get; }
    }
}
