// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExRam.ReactiveCollections
{
    public abstract class ReactiveCollectionSource<TNotification>
        where TNotification : ICollectionChangedNotification
    {
        #region ReactiveCollectionImpl
        private sealed class ReactiveCollectionImpl : IReactiveCollection<TNotification>
        {
            private readonly IObservable<TNotification> _changes;

            public ReactiveCollectionImpl(IObservable<TNotification> subject)
            {
                Contract.Requires(subject != null);

                this._changes = subject
                    .Normalize();
            }

            IObservable<TNotification> IReactiveCollection<TNotification>.Changes => this._changes;
        }
        #endregion

        private readonly BehaviorSubject<TNotification> _subject;
        private readonly IReactiveCollection<TNotification> _reactiveCollection;

        protected ReactiveCollectionSource(TNotification initialNotification)
        {
            // ReSharper disable RedundantCast
            Contract.Requires(((object)initialNotification) != null);
            // ReSharper restore RedundantCast

            this._subject = new BehaviorSubject<TNotification>(initialNotification);
            this._reactiveCollection = new ReactiveCollectionImpl(this._subject);
        }

        public IReactiveCollection<TNotification> ReactiveCollection
        {
            get
            {
                Contract.Ensures(Contract.Result<IReactiveCollection<TNotification>>() != null);

                return this._reactiveCollection;
            }
        }

        protected BehaviorSubject<TNotification> Subject
        {
            get
            {
                Contract.Ensures(Contract.Result<BehaviorSubject<TNotification>>() != null);

                return this._subject;
            }
        }
    }
}
