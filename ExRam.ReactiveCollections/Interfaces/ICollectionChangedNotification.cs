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
    [ContractClass(typeof(CollectionChangedNotificationContracts<>))]
    public interface ICollectionChangedNotification<out T> 
    {
        ICollectionChangedNotification<T> ToResetNotification();

        IReadOnlyList<T> OldItems
        {
            get;
        }

        IReadOnlyList<T> NewItems
        {
            get;
        }

        NotifyCollectionChangedAction Action
        {
            get;
        }

        IReadOnlyCollection<T> Current
        {
            get;
        }
    }

    [ContractClassFor(typeof(ICollectionChangedNotification<>))]
    public abstract class CollectionChangedNotificationContracts<T> : ICollectionChangedNotification<T>
    {
        public ICollectionChangedNotification<T> ToResetNotification()
        {
            Contract.Ensures(Contract.Result<ICollectionChangedNotification<T>>() != null);

            return default(ICollectionChangedNotification<T>);
        }

        public IReadOnlyList<T> OldItems
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);

                return default(IReadOnlyList<T>);
            }
        }

        public IReadOnlyList<T> NewItems
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyList<T>>() != null);

                return default(IReadOnlyList<T>);
            }
        }

        public abstract NotifyCollectionChangedAction Action
        {
            get;
        }

        public IReadOnlyCollection<T> Current
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyCollection<T>>() != null);

                return default(IReadOnlyCollection<T>);
            }
        }
    }
}
