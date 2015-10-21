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
    [ContractClass(typeof(CollectionChangedNotificationContracts))]
    public interface ICollectionChangedNotification
    {
        ICollectionChangedNotification ToResetNotification();

        IEnumerable OldItems
        {
            get;
        }

        IEnumerable NewItems
        {
            get;
        }

        NotifyCollectionChangedAction Action
        {
            get;
        }

        IEnumerable Current
        {
            get;
        }
    }

    [ContractClass(typeof(CollectionChangedNotificationContracts<>))]
    public interface ICollectionChangedNotification<out T> : ICollectionChangedNotification
    {
        new ICollectionChangedNotification<T> ToResetNotification();

        new IReadOnlyList<T> OldItems
        {
            get;
        }

        new IReadOnlyList<T> NewItems
        {
            get;
        }

        new IReadOnlyCollection<T> Current
        {
            get;
        }
    }

    [ContractClassFor(typeof(ICollectionChangedNotification))]
    public abstract class CollectionChangedNotificationContracts : ICollectionChangedNotification
    {
        ICollectionChangedNotification ICollectionChangedNotification.ToResetNotification()
        {
            Contract.Ensures(Contract.Result<ICollectionChangedNotification>() != null);

            return default(ICollectionChangedNotification);
        }

        IEnumerable ICollectionChangedNotification.NewItems
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable>() != null);

                return default(IEnumerable);
            }
        }

        NotifyCollectionChangedAction ICollectionChangedNotification.Action
        {
            get
            {
                return default(NotifyCollectionChangedAction);
            }
        }

        IEnumerable ICollectionChangedNotification.OldItems
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable>() != null);

                return default(IEnumerable);
            }
        }

        IEnumerable ICollectionChangedNotification.Current
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable>() != null);

                return default(IEnumerable);
            }
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

        IEnumerable ICollectionChangedNotification.Current
        {
            get { return Current; }
        }

        public IReadOnlyCollection<T> Current
        {
            get
            {
                Contract.Ensures(Contract.Result<IReadOnlyCollection<T>>() != null);

                return default(IReadOnlyCollection<T>);
            }
        }

        IEnumerable ICollectionChangedNotification.OldItems
        {
            get
            {
                return default(IEnumerable<T>);
            }
        }

        IEnumerable ICollectionChangedNotification.NewItems
        {
            get
            {
                return default(IEnumerable<T>);
            }
        }

        ICollectionChangedNotification ICollectionChangedNotification.ToResetNotification()
        {
            return default(ICollectionChangedNotification);
        }
    }
}
