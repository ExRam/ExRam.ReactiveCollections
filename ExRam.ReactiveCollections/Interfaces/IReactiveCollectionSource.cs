// (c) Copyright 2014 ExRam GmbH & Co. KG http://www.exram.de
//
// Licensed using Microsoft Public License (Ms-PL)
// Full License description can be found in the LICENSE
// file.

using System.Diagnostics.Contracts;

namespace ExRam.ReactiveCollections
{
    [ContractClass(typeof(ReactiveCollectionSourceContracts<,>))]
    public interface IReactiveCollectionSource<out TNotification, T>
         where TNotification : ICollectionChangedNotification<T>
    {
        IReactiveCollection<TNotification, T> ReactiveCollection
        {
            get;
        }
    }

    [ContractClassFor(typeof(IReactiveCollectionSource<,>))]
    public abstract class ReactiveCollectionSourceContracts<TNotification, T> : IReactiveCollectionSource<TNotification, T>
        where TNotification : ICollectionChangedNotification<T>
    {
        IReactiveCollection<TNotification, T> IReactiveCollectionSource<TNotification, T>.ReactiveCollection
        {
            get 
            {
                Contract.Ensures(Contract.Result<IReactiveCollection<TNotification, T>>() != null);

                return default(IReactiveCollection<TNotification, T>);
            }
        }
    }
}
