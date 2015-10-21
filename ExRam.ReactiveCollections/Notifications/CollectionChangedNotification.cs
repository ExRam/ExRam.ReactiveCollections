// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    public abstract class CollectionChangedNotification<T> : ICollectionChangedNotification<T>
    {
        private readonly IReadOnlyCollection<T> _current;

        protected CollectionChangedNotification(IReadOnlyCollection<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems)
        {
            // ReSharper disable RedundantCast
            Contract.Requires(((object)current) != null);
            // ReSharper restore RedundantCast
            Contract.Requires(oldItems != null);
            Contract.Requires(newItems != null);

            this.Action = action;
            this._current = current;
            this.OldItems = oldItems;
            this.NewItems = newItems;
        }

        public abstract ICollectionChangedNotification<T> ToResetNotification();

        public IReadOnlyList<T> OldItems { get; }

        public IReadOnlyList<T> NewItems { get; }

        public NotifyCollectionChangedAction Action { get; }

        public IReadOnlyCollection<T> Current
        {
            get 
            {
                // ReSharper disable RedundantCast
                Contract.Ensures(((object)Contract.Result<IReadOnlyCollection<T>>()) != null);
                Contract.Assume(((object)this._current) != null);
                // ReSharper restore RedundantCast

                return this._current;
            }
        }

        ICollectionChangedNotification ICollectionChangedNotification.ToResetNotification()
        {
            return this.ToResetNotification();
        }

        IEnumerable ICollectionChangedNotification.Current => this.Current;

        IEnumerable ICollectionChangedNotification.NewItems => this.NewItems;

        IEnumerable ICollectionChangedNotification.OldItems => this.OldItems;
    }
}
