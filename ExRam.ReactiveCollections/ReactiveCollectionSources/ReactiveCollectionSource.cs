using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExRam.ReactiveCollections
{
    public abstract class ReactiveCollectionSource<TNotification, T>
        where TNotification : ICollectionChangedNotification<T>
    {
        #region ReactiveCollectionImpl
        private sealed class ReactiveCollectionImpl : IReactiveCollection<TNotification, T>
        {
            private readonly IObservable<TNotification> _changes;

            public ReactiveCollectionImpl(IObservable<TNotification> subject)
            {
                Contract.Requires(subject != null);

                this._changes = subject
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

        private readonly IReactiveCollection<TNotification, T> _reactiveCollection;
        private readonly BehaviorSubject<TNotification> _subject = new BehaviorSubject<TNotification>(default(TNotification));

        protected ReactiveCollectionSource()
        {
            this._reactiveCollection = new ReactiveCollectionImpl(this._subject);
        }

        public IReactiveCollection<TNotification, T> ReactiveCollection
        {
            get
            {
                return this._reactiveCollection;
            }
        }

        protected BehaviorSubject<TNotification> Subject
        {
            get
            {
                return this._subject;
            }
        }
    }
}
