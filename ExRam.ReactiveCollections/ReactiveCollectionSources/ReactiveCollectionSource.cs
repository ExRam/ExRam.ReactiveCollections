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

        private readonly BehaviorSubject<TNotification> _subject;
        private readonly IReactiveCollection<TNotification> _reactiveCollection;

        protected ReactiveCollectionSource([NotNull] TNotification initialNotification)
        {
            this._subject = new BehaviorSubject<TNotification>(initialNotification);
            this._reactiveCollection = new ReactiveCollectionImpl(this._subject);
        }

        [NotNull]
        public IReactiveCollection<TNotification> ReactiveCollection
        {
            get
            {
                return this._reactiveCollection;
            }
        }

        [NotNull]
        protected BehaviorSubject<TNotification> Subject
        {
            get
            {
                return this._subject;
            }
        }
    }
}
