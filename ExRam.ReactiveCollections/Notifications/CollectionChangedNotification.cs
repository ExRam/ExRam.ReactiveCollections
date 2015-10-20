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
    public abstract class CollectionChangedNotification<T> : ICollectionChangedNotification<T>
    {
        private readonly IReadOnlyList<T> _oldItems;
        private readonly IReadOnlyList<T> _newItems;
        private readonly IReadOnlyCollection<T> _current;
        private readonly NotifyCollectionChangedAction _action;

        protected CollectionChangedNotification(IReadOnlyCollection<T> current, NotifyCollectionChangedAction action, IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems)
        {
            // ReSharper disable RedundantCast
            Contract.Requires(((object)current) != null);
            // ReSharper restore RedundantCast
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

        public NotifyCollectionChangedAction Action
        {
            get 
            {
                return this._action; 
            }
        }

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
    }
}
