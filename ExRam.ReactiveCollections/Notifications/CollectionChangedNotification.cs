// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    public abstract class CollectionChangedNotification<TCollection, T> : ICollectionChangedNotification<T>
        where TCollection : IReadOnlyCollection<T>
    {
        private readonly TCollection _current;
        private readonly IReadOnlyList<T> _oldItems;
        private readonly IReadOnlyList<T> _newItems;
        private readonly NotifyCollectionChangedAction _action;

        protected CollectionChangedNotification(TCollection current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems)
        {
            Contract.Requires(current != null);
            Contract.Requires(oldItems != null);
            Contract.Requires(newItems != null);

            this._action = action;
            this._current = current;
            this._oldItems = oldItems;
            this._newItems = newItems;
        }

        public abstract ICollectionChangedNotification<T> ToResetNotification();

        public IReadOnlyList<T> OldItems
        {
            get 
            {
                return this._oldItems;
            }
        }

        public IReadOnlyList<T> NewItems
        {
            get
            {
                return this._newItems;
            }
        }

        public TCollection Current
        {
            get 
            {
                Contract.Ensures(Contract.Result<TCollection>() != null);

                Contract.Assume(this._current != null);
                return this._current;
            }
        }

        public NotifyCollectionChangedAction Action
        {
            get 
            {
                return this._action; 
            }
        }

        IReadOnlyCollection<T> ICollectionChangedNotification<T>.Current
        {
            get 
            {
                return this.Current;
            }
        }
    }
}
