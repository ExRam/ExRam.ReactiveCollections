// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    [ContractClass(typeof(ReactiveCollectionSourceContracts<>))]
    public interface IReactiveCollectionSource<out TNotification>
         where TNotification : ICollectionChangedNotification
    {
        IReactiveCollection<TNotification> ReactiveCollection
        {
            get;
        }
    }

    [ContractClassFor(typeof(IReactiveCollectionSource<>))]
    public abstract class ReactiveCollectionSourceContracts<TNotification> : IReactiveCollectionSource<TNotification>
        where TNotification : ICollectionChangedNotification
    {
        IReactiveCollection<TNotification> IReactiveCollectionSource<TNotification>.ReactiveCollection
        {
            get 
            {
                Contract.Ensures(Contract.Result<IReactiveCollection<TNotification>>() != null);

                return default(IReactiveCollection<TNotification>);
            }
        }
    }
}
