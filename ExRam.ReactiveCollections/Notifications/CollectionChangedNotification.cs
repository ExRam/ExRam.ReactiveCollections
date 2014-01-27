using System.Collections.Generic;
using System.Collections.Immutable;
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
            Contract.Requires((object)current != null);
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
                Contract.Ensures((object)Contract.Result<TCollection>() != null);

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
